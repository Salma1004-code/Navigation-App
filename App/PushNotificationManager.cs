using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using UserNotifications;
using WindowsAzure.Messaging.NotificationHubs;

public class PushNotificationManager : NSObject, IUNUserNotificationCenterDelegate
{
    // Constants
    private static class NotificationKeys
    {
        public static string PushNotification = "GDAPushNotification";
    }

    private static class InfoPlistKeys
    {
        public static class AzureNHConnectionString
        {
            public const string Default = "SOUNDSCAPE_AZURE_NH_CONNECTION_STRING";
            public const string Dogfood = "SOUNDSCAPE_AZURE_DF_NH_CONNECTION_STRING";
        }

        public static class AzureNHPath
        {
            public const string Default = "SOUNDSCAPE_AZURE_NH_PATH";
            public const string Dogfood = "SOUNDSCAPE_AZURE_DF_NH_PATH";
        }
    }

    // Properties
    private static Dictionary<string, string> GlobalTags
    {
        get
        {
            Dictionary<string, string> tags = new Dictionary<string, string>
            {
                { "device.model", UIDevice.CurrentDevice.Model },
                { "device.os.version", UIDevice.CurrentDevice.SystemVersion },
                { "device.voice_over", UIAccessibility.IsVoiceOverRunning ? "on" : "off" },
                { "app.version", AppContext.AppVersion },
                { "app.build", AppContext.AppBuild },
                { "app.source", BuildSettings.Source.ToString() },
                { "app.language", LocalizationContext.CurrentLanguageCode },
                { "app.region", LocalizationContext.CurrentRegionCode }
            };

            foreach (var key in tags.Keys.ToList())
            {
                tags[key] = tags[key].ToLower().Replace(" ", "-");
            }

            return tags;
        }
    }

    private static string AzureConnectionString
    {
        get
        {
            string key = BuildSettings.Source == BuildSource.AppCenter ?
                InfoPlistKeys.AzureNHConnectionString.Dogfood :
                InfoPlistKeys.AzureNHConnectionString.Default;

            string connectionString = NSBundle.MainBundle.InfoDictionary[key]?.ToString();
            if (string.IsNullOrEmpty(connectionString))
            {
                GDLogPushError($"Error: The Azure connection string was not found in the Info.plist file. Please set a valid value for key '{key}'");
            }

            return connectionString;
        }
    }

    private static string AzureNotificationHubPath
    {
        get
        {
            string key = BuildSettings.Source == BuildSource.AppCenter ?
                InfoPlistKeys.AzureNHPath.Dogfood :
                InfoPlistKeys.AzureNHPath.Default;

            string notificationHubPath = NSBundle.MainBundle.InfoDictionary[key]?.ToString();
            if (string.IsNullOrEmpty(notificationHubPath))
            {
                GDLogPushError($"Error: The Azure notification hub path was not found in the Info.plist file. Please set a valid value for key '{key}'");
            }

            return notificationHubPath;
        }
    }

    private string userId;
    private List<IDisposable> subscribers = new List<IDisposable>();
    private bool onboardingDidComplete = false;
    private bool appDidInitialize = false;
    private PushNotification pendingPushNotification;
    private LocalPushNotificationManager localPushNotificationManager = new LocalPushNotificationManager();
    private Action<UNNotificationPresentationOptions> notificationPresentationCompletion;
    private Action notificationResponseCompletion;

    // Initialization
    public PushNotificationManager(string userId = null)
    {
        UNUserNotificationCenter.Current.Delegate = this;
        MSNotificationHub.SetDelegate(this);
        MSNotificationHub.SetLifecycleDelegate(this);
        MSNotificationHub.SetEnrichmentDelegate(this);

        this.userId = userId;
        this.onboardingDidComplete = FirstUseExperience.DidComplete(FirstUseExperience.Type.oobe);

        NSNotificationCenter.DefaultCenter.AddObserver(
            NSNotification.Name.AppDidInitialize,
            onAppDidInitialize,
            null);

        if (!onboardingDidComplete)
        {
            NSNotificationCenter.DefaultCenter.AddObserver(
                NSNotification.OnboardingDidComplete,
                onOnboardingDidComplete,
                null);
        }

        subscribers.Add(
            NSNotificationCenter.DefaultCenter.AddObserver(
                NSNotification.DidRegisterForRemoteNotifications,
                (NSNotification notification) =>
                {
                    if (userId != null)
                    {
                        UpdateUserIdIfNeeded(userId);
                    }
                    UpdateTagsIfNeeded();
                },
                null));
    }

    ~PushNotificationManager()
    {
        foreach (var subscriber in subscribers)
        {
            subscriber.Dispose();
        }
        subscribers.Clear();
    }

    // Methods
    public void Start()
    {
        if (!onboardingDidComplete)
        {
            return;
        }

        string connectionString = AzureConnectionString;
        string hubName = AzureNotificationHubPath;

        if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(hubName))
        {
            GDLogPushError("Error: The Azure notification hub connection string or hub name was not found in the Info.plist file");
            return;
        }

        MSNotificationHub.Start(connectionString, hubName);
    }

    private void UpdateUserIdIfNeeded(string userId)
    {
        if (userId != MSNotificationHub.GetUserId())
        {
            MSNotificationHub.SetUserId(userId);
        }
    }

    private void UpdateTagsIfNeeded()
    {
        var globalTags = GlobalTags;
        var currentTags = MSNotificationHub.GetTags();
        var tagsToRemove = new List<string>();
        var tagsToAdd = new List<string>();

        foreach (var globalTag in globalTags)
        {
            var globalTagKey = globalTag.Key;
            var globalTagValue = globalTag.Value;
            var globalTagString = $"{globalTagKey}:{globalTagValue}";

            var currentTag = currentTags.FirstOrDefault(tag => tag.StartsWith(globalTagKey));
            if (currentTag == null)
            {
                tagsToAdd.Add(globalTagString);
                continue;
            }

            var parts = currentTag.Split(':');
            if             (parts.Length == 2)
            {
                var currentTagValue = parts[1];

                if (currentTagValue != globalTagValue)
                {
                    tagsToRemove.Add(currentTag);
                    tagsToAdd.Add(globalTagString);
                }
            }
        }

        if (tagsToRemove.Count > 0)
        {
            GDLogPushInfo($"Removing tags: {string.Join(", ", tagsToRemove)}");
            MSNotificationHub.RemoveTags(tagsToRemove.ToArray());
        }

        if (tagsToAdd.Count > 0)
        {
            GDLogPushInfo($"Adding tags: {string.Join(", ", tagsToAdd)}");
            MSNotificationHub.AddTags(tagsToAdd.ToArray());
        }
    }

    public void DidFinishLaunchingWithOptions(NSDictionary launchOptions)
    {
        if (launchOptions.ContainsKey(UIApplication.LaunchOptionsRemoteNotificationKey) &&
            launchOptions[UIApplication.LaunchOptionsRemoteNotificationKey] is NSDictionary remoteNotification)
        {
            var payload = remoteNotification as PushNotification.Payload;
            var pushNotification = new PushNotification(payload, PushNotification.ArrivalContext.Launch);
            DidReceive(pushNotification);
        }
    }

    public void DidReceive(PushNotification pushNotification)
    {
        if (!appDidInitialize)
        {
            pendingPushNotification = pushNotification;
            GDLogPushInfo("Did receive pending push notification");
            return;
        }

        GDLogPushInfo($"Did receive push notification. App state: {UIApplication.SharedApplication.ApplicationState}, " +
                      $"Origin context: {pushNotification.OriginContext}, Arrival context: {pushNotification.ArrivalContext}, " +
                      $"Payload: {pushNotification.Payload}");

        GDATelemetry.Track("push.received_notification", new Dictionary<string, string>
        {
            { "origin_context", pushNotification.OriginContext.ToString() },
            { "arrival_context", pushNotification.ArrivalContext.ToString() },
            { "local_identifier", pushNotification.LocalIdentifier ?? "none" }
        });

        NSNotificationCenter.DefaultCenter.PostNotificationName(
            NotificationKeys.PushNotificationReceived,
            this,
            new NSDictionary(NotificationKeys.PushNotification, pushNotification));
    }

    // Notification Handling
    private void OnOnboardingDidComplete(NSNotification notification)
    {
        onboardingDidComplete = true;
        NSNotificationCenter.DefaultCenter.RemoveObserver(this, NSNotification.OnboardingDidComplete, null);

        Start();

        if (appDidInitialize && pendingPushNotification != null)
        {
            DidReceive(pendingPushNotification);
            pendingPushNotification = null;
        }
    }

    private void OnAppDidInitialize(NSNotification notification)
    {
        appDidInitialize = true;
        NSNotificationCenter.DefaultCenter.RemoveObserver(this, NSNotification.AppDidInitialize, null);

        if (onboardingDidComplete && pendingPushNotification != null)
        {
            DidReceive(pendingPushNotification);
            pendingPushNotification = null;
        }
    }

    // UNUserNotificationCenterDelegate
    [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
    public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
    {
        notificationPresentationCompletion = completionHandler;
    }

    [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
    public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
    {
        notificationResponseCompletion = completionHandler;
    }
}


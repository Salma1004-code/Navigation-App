using Foundation;
using UIKit;
using System;
using System.Linq;

[Register("AppDelegate")]
public class AppDelegate : UIResponder, IUIApplicationDelegate
{
    UIWindow window;
    UserActivityManager userActivityManager = new UserActivityManager();
    URLResourceManager urlResourceManager = new URLResourceManager();
    PushNotificationManager pushNotificationManager;

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        RealmMigrationTools.Migrate(RealmHelper.DatabaseConfig, RealmHelper.CacheConfig);

        if (FirstUseExperience.DidComplete(FirstUseExperienceEnum.OOBE))
        {
            SettingsContext.Shared.AppUseCount++;
        }

        // Initialize push notification manager
        pushNotificationManager = new PushNotificationManager(SettingsContext.Shared.ClientId);
        pushNotificationManager.DidFinishLaunchingWithOptions(launchOptions);

        return true;
    }

    public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
    {
        var components = new NSUrlComponents(url, true);
        var sourceQueryItem = components.QueryItems?.FirstOrDefault(item => item.Name == "source");
        var source = sourceQueryItem?.Value;
        Console.WriteLine($"App opened from source: {source}");
        GDATelemetry.Track("app.open", with: new NSDictionary("source", source));
        return urlResourceManager.OnOpenResource(url);
    }

    public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
    {
        GDLogPushInfo("Did register for remote notifications");
        NSNotificationCenter.DefaultCenter.Post(NotificationKeys.DidRegisterForRemoteNotifications, this);
    }

    public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
    {
        GDLogPushInfo($"Did fail to register for remote notifications with error: {error}");
    }

    public override void WillResignActive(UIApplication application)
    {
        GDLogAppInfo("Application will resign active");
        AppContext.AppState = AppState.Inactive;
    }

    public override void DidEnterBackground(UIApplication application)
    {
        GDLogAppInfo("Application did enter background");
        AppContext.AppState = AppState.Background;
        NSNotificationCenter.DefaultCenter.Post(NotificationKeys.AppDidEnterBackground, this);
    }

    public override void WillEnterForeground(UIApplication application)
    {
        GDLogAppInfo("Application will enter foreground");
        AppContext.AppState = AppState.Inactive;
        NSNotificationCenter.DefaultCenter.Post(NotificationKeys.AppWillEnterForeground, this);
    }

    public override void DidBecomeActive(UIApplication application)
    {
        GDLogAppInfo("Application did become active");
        AppContext.AppState = AppState.Active;
        AppContext.Shared.ValidateActive();
        NSNotificationCenter.DefaultCenter.Post(NotificationKeys.AppDidBecomeActive, this);
    }

    public override void WillTerminate(UIApplication application)
    {
        GDLogAppInfo("Application will terminate");
        if (AppContext.Shared.GeolocationManager.IsTracking)
        {
            AppContext.Shared.GeolocationManager.StopTrackingGPX();
        }
    }

    public override void DidReceiveMemoryWarning(UIApplication application)
    {
        GDLogAppInfo("Application did receive memory warning");
        var memoryAllocated = AppContext.MemoryAllocated;
        if (memoryAllocated != null)
        {
            GDLogAppInfo($"Memory used: {ByteCountFormatter.StringFromByteCount((long)memoryAllocated, CountStyle.Memory)}");
        }
    }

    public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
    {
        return userActivityManager.OnContinueUserActivity(userActivity);
    }

    public override void DidRegisterUserNotificationSettings(UIApplication application, UIUserNotificationSettings notificationSettings)
    {
        UIApplication.SharedApplication.RegisterForRemoteNotifications();
    }

    public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
    {
        pushNotificationManager.DidReceiveRemoteNotification(userInfo, completionHandler);
    }

    public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
    {
        pushNotificationManager.PerformFetch(completionHandler);
    }

    public override void HandleAction(UIApplication application, string actionIdentifier, NSDictionary remoteNotificationInfo, Action completionHandler)
    {
        pushNotificationManager.HandleAction(actionIdentifier, remoteNotificationInfo, completionHandler);
    }

    public override void HandleEventsForBackgroundUrl(UIApplication application, string sessionIdentifier, Action completionHandler)
    {
        pushNotificationManager.HandleEventsForBackgroundUrl(sessionIdentifier, completionHandler);
    }

    public override void HandleWatchKitExtensionRequest(UIApplication application, NSDictionary userInfo, Action<NSDictionary> reply)
    {
        pushNotificationManager.HandleWatchKitExtensionRequest(userInfo, reply);
    }
}

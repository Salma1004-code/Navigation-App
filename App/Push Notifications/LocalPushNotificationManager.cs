using Foundation;
using UserNotifications;
using System;
using System.Collections.Generic;
using UIKit;

public enum NotificationIdentifier
{
    ReminderHome,
    ReminderWalk
}

public class LocalPushNotificationManager
{
    private UNUserNotificationCenter notificationCenter = UNUserNotificationCenter.Current;
    private List<UNNotificationRequest> pendingNotificationRequests = new List<UNNotificationRequest>();

    private NSUserDefaults UserDefaults
    {
        get { return NSUserDefaults.StandardUserDefaults; }
    }

    public LocalPushNotificationManager()
    {
        if (!DidSetBeacon)
        {
            NSNotificationCenter.DefaultCenter.AddObserver(this, new ObjCRuntime.Selector("OnBeaconStarted"), new NSString("destinationChanged"), null);
        }

        if (!DidWalkWithApp)
        {
            NSNotificationCenter.DefaultCenter.AddObserver(this, new ObjCRuntime.Selector("OnMotionActivityDidChange:"), new NSString("motionActivityDidChange"), null);
        }
    }

    public void Start()
    {
        RefreshNotifications();
    }

    public void RefreshNotifications()
    {
        UpdatePendingNotifications(() =>
        {
            foreach (var pendingNotificationRequest in pendingNotificationRequests)
            {
                Console.WriteLine($"Pending local notification: {pendingNotificationRequest.Identifier}");
            }

            UnscheduleNotificationsIfNeeded();
            ScheduleNotificationsIfNeeded();
        });
    }

    private void UpdatePendingNotifications(Action completion = null)
    {
        notificationCenter.GetPendingNotificationRequests(requests =>
        {
            pendingNotificationRequests = new List<UNNotificationRequest>(requests);
            completion?.Invoke();
        });
    }

    private void UnscheduleNotificationsIfNeeded()
    {
        foreach (NotificationIdentifier localPushIdentifier in Enum.GetValues(typeof(NotificationIdentifier)))
        {
            if (ShouldUnschedule(localPushIdentifier))
            {
                Unschedule(localPushIdentifier);
            }
        }
    }

    private void ScheduleNotificationsIfNeeded()
    {
        foreach (NotificationIdentifier localPushIdentifier in Enum.GetValues(typeof(NotificationIdentifier)))
        {
            if (ShouldSchedule(localPushIdentifier))
            {
                Schedule(localPushIdentifier);
            }
        }
    }

    private void Schedule(NotificationIdentifier localPushIdentifier)
    {
        var request = NotificationRequest(localPushIdentifier);
        Schedule(request);
        SetScheduled(true, localPushIdentifier);
    }

    private void Schedule(UNNotificationRequest request)
    {
        notificationCenter.AddNotificationRequest(request, error =>
        {
            if (error != null)
            {
                Console.WriteLine($"Could not schedule local push notification. Error: {error.LocalizedDescription}");
            }
            else
            {
                Console.WriteLine($"Scheduled local push notification for identifier: {request.Identifier}");
            }

            UpdatePendingNotifications();
        });
    }

    private void Unschedule(NotificationIdentifier localPushIdentifier)
    {
        notificationCenter.RemovePendingNotificationRequests(new string[] { localPushIdentifier.ToString() });
        Console.WriteLine($"Unscheduled local push notification for identifier: {localPushIdentifier}");
        UpdatePendingNotifications();
    }

    public void ResetAll()
    {
        foreach (NotificationIdentifier localPushIdentifier in Enum.GetValues(typeof(NotificationIdentifier)))
        {
            SetScheduled(false, localPushIdentifier);
        }
    }

    private bool DidSchedule(NotificationIdentifier identifier)
    {
        return UserDefaults.BoolForKey(identifier.ToString());
    }

    private void SetScheduled(bool scheduled, NotificationIdentifier identifier)
    {
        UserDefaults.SetBool(scheduled, identifier.ToString());
    }

    private bool IsPending(NotificationIdentifier identifier)
    {
        return pendingNotificationRequests.Exists(request => request.Identifier == identifier.ToString());
    }

    private bool ShouldSchedule(NotificationIdentifier identifier)
    {
        switch (identifier)
        {
            case NotificationIdentifier.ReminderHome:
                return !DidSchedule(NotificationIdentifier.ReminderHome) && !DidSetBeacon;
            case NotificationIdentifier.ReminderWalk:
                return !DidSchedule(NotificationIdentifier.ReminderWalk) && !DidWalkWithApp;
            default:
                return false;
        }
    }

    private bool ShouldUnschedule(NotificationIdentifier identifier)
    {
        switch (identifier)
        {
            case NotificationIdentifier.ReminderHome:
                return IsPending(NotificationIdentifier.ReminderHome) && DidSetBeacon;
            case NotificationIdentifier.ReminderWalk:
                return IsPending(NotificationIdentifier.ReminderWalk) && DidWalkWithApp;
            default:
                return false;
        }
    }

    private UNNotificationRequest NotificationRequest(NotificationIdentifier identifier)
    {
        var content = new UNMutableNotificationContent();
        UNNotificationTrigger trigger;

        switch (identifier)
        {
            case NotificationIdentifier.ReminderHome:
                content.Title = GDLocalizedString("push.local.home.title");
                content.Body = GDLocalizedString("push.local.home.body");
                content.UserInfo = NSDictionary.FromObjectsAndKeys(new NSObject[] { new NSString(PushNotification.Keys.OriginContext), new NSString(PushNotification.OriginContext.Local.ToString()), new NSString(PushNotification.Keys.LocalIdentifier), new NSString(NotificationIdentifier.ReminderHome.ToString()) }, new NSObject[] { new NSString(DebugSettingsContext.Shared.LocalPushNotification1TimeInternal.ToString()), new NSString("false") });
                trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(DebugSettingsContext.Shared.LocalPushNotification1TimeInternal, false);
                break;
            case NotificationIdentifier.ReminderWalk:
                content.Title = GDLocalizedString("push.local.walk.title");
                content.Body = GDLocalizedString("push.local.walk.body");
                content.UserInfo = NSDictionary.FromObjectsAndKeys(new NSObject[] { new NSString(PushNotification.Keys.OriginContext), new NSString(PushNotification.OriginContext.Local.ToString()), new NSString(PushNotification.Keys.LocalIdentifier), new NSString(NotificationIdentifier.ReminderWalk.ToString()) }, new NSObject[] { new NSString(DebugSettingsContext.Shared.LocalPushNotification2TimeInternal.ToString()), new NSString("false") });
                trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(DebugSettingsContext.Shared.LocalPushNotification2TimeInternal, false);
                break;
            default:
                trigger = null;
                break;
        }

        return UNNotificationRequest.FromIdentifier(identifier.ToString(), content, trigger);
    }

    private bool DidSetBeacon
    {
        get
        {
            var beaconCountSet = GDATelemetry.Helper?.BeaconCountSet;
            return beaconCountSet.HasValue && beaconCountSet.Value > 0;
        }
    }

    private bool DidWalkWithApp
    {
        get { return GDATelemetry.Helper?.DidWalkWithApp ?? false; }
    }

    [Export("OnBeaconStarted")]
    private void OnBeaconStarted()
    {
        NSNotificationCenter.DefaultCenter.RemoveObserver(this, new NSString("destinationChanged"), null);
        RefreshNotifications();
    }

    [Export("OnMotionActivityDidChange:")]
    private void OnMotionActivityDidChange(NSNotification notification)
    {
        var activityType = (ActivityType)(NSNumber)notification.UserInfo[MotionActivityContext.NotificationKeys.ActivityType];
        if (activityType == ActivityType.Walking)
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(this, new NSString("motionActivityDidChange"), null);
            RefreshNotifications();
        }
    }
}

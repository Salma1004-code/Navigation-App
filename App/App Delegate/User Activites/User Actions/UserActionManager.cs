using System;
using System.Collections.Generic;

public class UserActionManager
{
    // Notification Keys
    private static class Keys
    {
        public const string UserAction = "GDAUserAction";
    }

    // Properties
    private UserAction? pendingUserAction;
    private bool homeViewControllerDidLoad;

    // Static Properties
    public static List<NSUserActivity> AppUserActions
    {
        get
        {
            var userActions = new List<NSUserActivity>();
            foreach (UserAction action in Enum.GetValues(typeof(UserAction)))
            {
                userActions.Add(new NSUserActivity(action));
            }
            return userActions;
        }
    }

    public static List<INShortcut> AppShortcuts
    {
        get
        {
            var appShortcuts = new List<INShortcut>();
            foreach (var userActivity in AppUserActions)
            {
                appShortcuts.Add(new INShortcut(userActivity));
            }
            return appShortcuts;
        }
    }

    // Initialization
    public UserActionManager()
    {
        NotificationCenter.DefaultCenter.AddObserver(
            this,
            new ObjCRuntime.Selector("OnHomeViewControllerDidLoad"),
            Notification.Name.HomeViewControllerDidLoad,
            null);

        NotificationCenter.DefaultCenter.AddObserver(
            this,
            new ObjCRuntime.Selector("OnPushNotificationReceived:"),
            Notification.Name.PushNotificationReceived,
            null);

        NotificationCenter.DefaultCenter.AddObserver(
            this,
            new ObjCRuntime.Selector("OnAppLocaleDidChange"),
            Notification.AppLocaleDidChange,
            null);

        if (!FirstUseExperience.DidComplete(FirstUseExperienceType.DonateSiriShortcuts))
        {
            DonateSiriShortcuts();
            FirstUseExperience.SetDidComplete(FirstUseExperienceType.DonateSiriShortcuts);
        }
    }

    // Methods
    private static void DonateSiriShortcuts()
    {
        var shortcuts = AppShortcuts;
        INVoiceShortcutCenter.Shared.SetShortcutSuggestions(shortcuts.ToArray());
        GDLogAppInfo($"Donated {shortcuts.Count} Siri shortcuts");
    }

    public bool ContinueUserAction(UserAction userAction)
    {
        if (!homeViewControllerDidLoad)
        {
            GDLogAppInfo($"Pending user action: {userAction}");
            pendingUserAction = userAction;
            return true;
        }

        if (AppContext.Shared.State != AppState.Normal)
        {
            AppContext.Shared.WakeUp();
        }

        GDATelemetry.Track("user_activity.perform", value: userAction.ToString());

        Action postNotification = () =>
        {
            var userInfo = new NSDictionary(Keys.UserAction, userAction.ToString());
            NotificationCenter.DefaultCenter.PostNotificationName(Notification.Name.ContinueUserAction, this, userInfo);
        };

        switch (userAction)
        {
            case UserAction.MyLocation:
            case UserAction.AroundMe:
            case UserAction.AheadOfMe:
            case UserAction.NearbyMarkers:
                postNotification();
                return true;
            case UserAction.Search:
                if (AppContext.Shared.IsStreetPreviewing)
                {
                    postNotification();
                    return true;
                }
                else
                {
                    return ActionSearch();
                }
            case UserAction.SaveMarker:
                return ActionSaveMarker();
            case UserAction.StreetPreview:
                return ActionStreetPreview();
            default:
                return false;
        }
    }

    private void ContinuePendingUserActionIfNeeded()
    {
        if (pendingUserAction != null)
        {
            ContinueUserAction(pendingUserAction.Value);
            pendingUserAction = null;
        }
    }

    // Notification Handlers
    [Export("OnHomeViewControllerDidLoad")]
    private void OnHomeViewControllerDidLoad()
    {
        homeViewControllerDidLoad = true;
        ContinuePendingUserActionIfNeeded();
    }

    [Export("OnPushNotificationReceived:")]
    private void OnPushNotificationReceived(NSNotification notification)
    {
        if (notification.UserInfo.TryGetValue(
            new NSString(PushNotificationManager.NotificationKeys.PushNotification),
            out NSObject pushNotificationObj) &&
            pushNotificationObj is PushNotification pushNotification &&
            pushNotification.UserAction != null)
        {
            ContinueUserAction(pushNotification.UserAction.Value);
        }
    }

    [Export("OnAppLocaleDidChange")]
    private void OnAppLocaleDidChange()
    {
        // Re-donate the Siri shortcuts to change the shortcuts locale.
                UserActionManager.DonateSiriShortcuts();
    }

    // Actions
    private bool ActionSearch()
    {
        if (UserActionManager.IsSearching)
        {
            return false;
        }

        var rootViewController = RootViewController;
        if (rootViewController == null)
        {
            return false;
        }

        var searchResultsTableViewController = SearchResultsTableViewController.InstantiateStandaloneConfiguration();
        if (searchResultsTableViewController == null)
        {
            return false;
        }

        rootViewController.PresentViewController(searchResultsTableViewController, true, null);
        return true;
    }

    private bool ActionSaveMarker()
    {
        var rootViewController = AppContext.RootViewController;
        if (rootViewController == null)
        {
            return false;
        }

        var location = AppContext.Shared.GeolocationManager.Location;
        if (location == null)
        {
            rootViewController.PresentViewController(ErrorAlerts.BuildLocationAlert(), true, null);
            return false;
        }

        var locationDetail = new LocationDetail(location, "current_location.user_activity.save");

        LocationDetail.FetchNameAndAddressIfNeeded(locationDetail, (updatedLocationDetail) =>
        {
            var config = new EditMarkerConfig(
                detail: updatedLocationDetail,
                context: "user_action",
                addOrUpdateAction: DismissesViewControllerAction,
                deleteAction: DismissesViewControllerAction,
                cancelAction: DismissesViewControllerAction,
                leftBarButtonItemIsHidden: false);

            var vc = new MarkerEditViewRepresentable(config).MakeViewController();
            if (vc != null)
            {
                var nav = new NavigationController(vc);
                nav.View.AccessibilityIgnoresInvertColors = true;
                rootViewController.PresentViewController(nav, true, null);
            }
        });

        return true;
    }

    private bool ActionStreetPreview()
    {
        if (AppContext.Shared.IsStreetPreviewing)
        {
            return false;
        }

        if (AppContext.Shared.IsRouteGuidanceActive)
        {
            return false;
        }

        var rootViewController = AppContext.RootViewController;
        if (rootViewController == null)
        {
            return false;
        }

        var location = AppContext.Shared.GeolocationManager.Location;
        if (location == null)
        {
            rootViewController.PresentViewController(ErrorAlerts.BuildLocationAlert(), true, null);
            return false;
        }

        var storyboard = UIStoryboard.FromName("Preview", null);
        if (storyboard == null)
        {
            return false;
        }

        var nav = storyboard.InstantiateInitialViewController() as NavigationController;
        if (nav == null)
        {
            return false;
        }

        var vc = nav.TopViewController as PreviewViewController;
        if (vc == null)
        {
            return false;
        }

        nav.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
        vc.LocationDetail = new LocationDetail(location, "current_location.user_activity.street_preview");
        rootViewController.PresentViewController(nav, true, null);

        return true;
    }

    // Helpers
    private UIViewController RootViewController
    {
        get
        {
            var rootViewController = AppContext.RootViewController;
            if (rootViewController == null)
            {
                return null;
            }

            var visibleViewController = rootViewController.VisiblePresentedViewController ?? rootViewController;

            if (visibleViewController is StandbyViewController)
            {
                // If the app is in a sleep or snooze state, the `rootViewController` will be `StandbyViewController`.
                // We don't want to present views on top of it, but on its `presentingViewController`, which should be `HomeViewController`.
                visibleViewController = visibleViewController.PresentingViewController ?? rootViewController;
            }

            return visibleViewController;
        }
    }

    private static bool IsSearching
    {
        get
        {
            var rootViewController = AppContext.RootViewController?.VisiblePresentedViewController as UINavigationController;
            return rootViewController?.TopViewController is SearchResultsTableViewController;
        }
    }
}


using System;
using System.Collections.Generic;

public class UniversalLinkManager
{
    private List<UniversalLinkComponents> pendingLinkComponents = new List<UniversalLinkComponents>();
    private bool homeViewControllerDidLoad = false;
    private object lockObject = new object();
    // Handlers
    private RecreationalActivityLinkHandler recreationalActivityLinkHandler = new RecreationalActivityLinkHandler();
    private ShareMarkerLinkHandler shareMarkerLinkHandler = new ShareMarkerLinkHandler();

    public UniversalLinkManager()
    {
        NotificationCenter.Default.AddObserver(this, new Selector("OnHomeViewControllerDidLoad"), Notification.Name.HomeViewControllerDidLoad, null);
    }

    [Export("OnHomeViewControllerDidLoad")]
    private void OnHomeViewControllerDidLoad()
    {
        lock (lockObject)
        {
            homeViewControllerDidLoad = true;

            foreach (var components in pendingLinkComponents)
            {
                LaunchWithUniversalLink(components);
            }

            pendingLinkComponents.Clear();
        }
    }

    public bool OnLaunchWithUniversalLink(Uri url)
    {
        var components = UniversalLinkComponents.FromUrl(url);
        if (components == null)
        {
            // Failed to parse URL for universal link
            GDATelemetry.Track("deeplink.unsupported");
            // Notify iOS that the link was not handled by
            // the app
            return false;
        }

        lock (lockObject)
        {
            if (homeViewControllerDidLoad)
            {
                LaunchWithUniversalLink(components);
            }
            else
            {
                              // App is not initialized (e.g. first launch experience
                // is in progress) Add to array of universal links
                // pending action
                pendingLinkComponents.Add(components);
            }
        }

        // Notify iOS that link will be handled by the
        // app
        //
        // If the app fails to act on the link, the app is
        // now responsible for displaying the appropriate
        // failure notifications
        return true;
    }

    private void LaunchWithUniversalLink(UniversalLinkComponents components)
    {
        UniversalLinkHandler handler;

        var version = components.PathComponents.Version;
        var path = components.PathComponents.Path;

        switch (path)
        {
            case UniversalLinkPath.Experience:
                GDATelemetry.Track("deeplink.experiences");
                handler = recreationalActivityLinkHandler;
                break;
            case UniversalLinkPath.ShareMarker:
                GDATelemetry.Track("deeplink.share_marker");
                handler = shareMarkerLinkHandler;
                break;
            default:
                // Handle default case or log error
                handler = null;
                break;
        }

        // Dispatch to the appropriate universal link
        // handler
        if (handler != null)
        {
            handler.HandleUniversalLink(components.QueryItems, version);
        }
    }
}

public static class UniversalLinkManagerExtensions
{
    public static Uri ShareMarker(ReferenceEntity marker)
    {
        var parameters = MarkerParameters.FromMarker(marker);
        return UniversalLinkComponents.FromPathAndParameters(UniversalLinkPath.ShareMarker, parameters)?.Url;
    }

    public static Uri ShareEntity(POI entity)
    {
        var parameters = MarkerParameters.FromEntity(entity);
        return UniversalLinkComponents.FromPathAndParameters(UniversalLinkPath.ShareMarker, parameters)?.Url;
    }

    public static Uri ShareLocation(LocationDetail detail)
    {
        var parameters = MarkerParameters.FromLocation(detail);
        return UniversalLinkComponents.FromPathAndParameters(UniversalLinkPath.ShareMarker, parameters)?.Url;
    }

    public static Uri ShareLocation(string name, double latitude, double longitude)
    {
        var parameters = MarkerParameters.FromNameAndCoordinates(name, latitude, longitude);
        return UniversalLinkComponents.FromPathAndParameters(UniversalLinkPath.ShareMarker, parameters)?.Url;
    }
}


              

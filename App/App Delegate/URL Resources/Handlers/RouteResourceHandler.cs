using System;
using Foundation; // for NotificationCenter
using ObjCRuntime; // for Notification

public static class NotificationNames
{
    public static NSString DidImportRoute = new NSString("DidImportRoute");
    public static NSString DidFailToImportRoute = new NSString("DidFailToImportRoute");
}

public class RouteResourceHandler : URLResourceHandler
{
    private static class Keys
    {
        public const string Route = "Route";
    }

    public override void HandleURLResource(NSUrl url)
    {
        var parameters = RouteParameters.DecodeFrom(url);
        if (parameters != null)
        {
            var handler = new RouteParametersHandler();
            
            handler.MakeRoute(parameters, (result) =>
            {
                if (result.IsSuccess)
                {
                    DidImportRoute(result.Value);
                }
                else
                {
                    DidFailToImportRoute();
                }
            });
        }
        else
        {
            DidFailToImportRoute();
        }
    }

    private void DidImportRoute(Route route)
    {
        var userInfo = new NSMutableDictionary();
        userInfo.Add(Keys.Route, route);
        
        DispatchQueue.MainQueue.DispatchAsync(() =>
        {
            NotificationCenter.DefaultCenter.PostNotificationName(NotificationNames.DidImportRoute, this, userInfo);
        });
    }

    private void DidFailToImportRoute()
    {
        DispatchQueue.MainQueue.DispatchAsync(() =>
        {
            NotificationCenter.DefaultCenter.PostNotificationName(NotificationNames.DidFailToImportRoute, this);
        });
    }
}

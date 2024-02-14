using System;
using Foundation; // for NotificationCenter
using ObjCRuntime; // for Notification

public static class NotificationNames
{
    public static NSString DidImportGPXResource = new NSString("DidImportGPXResource");
}

public class GPXResourceHandler : URLResourceHandler
{
    private static class Keys
    {
        public const string Filename = "Filename";
        public const string Error = "Error";
    }

    public override void HandleURLResource(NSUrl url)
    {
        if (!FeatureFlag.IsEnabled(FeatureFlag.DeveloperTools))
        {
            Console.WriteLine("Developer tools are not enabled in this build");
            return;
        }

        try
        {
            GPXFileManager.Import(url);
            PostNotification(url, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not import GPX file with error: {ex.Message}");
            PostNotification(url, ex);
        }
    }

    private void PostNotification(NSUrl url, Exception error)
    {
        var userInfo = new NSMutableDictionary();
        userInfo.Add(Keys.Filename, url.LastPathComponent);

        if (error != null)
        {
            userInfo.Add(Keys.Error, error.ToString());
        }

        NotificationCenter.DefaultCenter.PostNotificationName(NotificationNames.DidImportGPXResource, this, userInfo);
    }
}

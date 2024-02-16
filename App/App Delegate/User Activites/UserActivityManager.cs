using Foundation;
using UIKit;

public class UserActivityManager
{
    // Properties
    private UniversalLinkManager universalLinkManager = new UniversalLinkManager();
    private UserActionManager userActionManager = new UserActionManager();

    // Manage User Activities
    public bool OnContinueUserActivity(NSUserActivity userActivity)
    {
        if (userActivity.ActivityType == NSUserActivityType.BrowsingWeb)
        {
            return ContinueBrowsingWebUserActivity(userActivity);
        }
        else if (userActivity.IsUserAction)
        {
            return ContinueUserActionActivity(userActivity);
        }
        else
        {
            LogError("NSUserActivity is not supported");
            // Notify iOS that the user activity was not handled by the app
            return false;
        }
    }

    private bool ContinueBrowsingWebUserActivity(NSUserActivity userActivity)
    {
        if (userActivity.WebpageUrl == null)
        {
            LogError("Webpage URL is expected for NSUserActivityType.BrowsingWeb");
            // Notify iOS that the user activity was not handled by the app
            return false;
        }

        return universalLinkManager.OnLaunchWithUniversalLink(userActivity.WebpageUrl);
    }

    private bool ContinueUserActionActivity(NSUserActivity userActivity)
    {
        var userAction = new UserAction(userActivity);
        if (userAction == null)
        {
            LogError("NSUserActivity is expected to be a UserAction type");
            return false;
        }

        return userActionManager.ContinueUserAction(userAction);
    }

    private void LogError(string errorMessage)
    {
        Console.WriteLine($"Error: {errorMessage}");
        // Log error to your preferred logging framework here
    }
}

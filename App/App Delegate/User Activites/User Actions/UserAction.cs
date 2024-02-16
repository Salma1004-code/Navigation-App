using System;
using System.Collections.Generic;

public enum UserAction
{
    MyLocation,
    AroundMe,
    AheadOfMe,
    NearbyMarkers,
    Search,
    SaveMarker,
    StreetPreview
}

public static class UserActionExtensions
{
    private static readonly Dictionary<UserAction, string> Titles = new Dictionary<UserAction, string>
    {
        { UserAction.MyLocation, "user_activity.my_location.title" },
        { UserAction.AroundMe, "user_activity.around_me.title" },
        { UserAction.AheadOfMe, "user_activity.ahead_of_me.title" },
        { UserAction.NearbyMarkers, "user_activity.nearby_markers.title" },
        { UserAction.Search, "user_activity.search.title" },
        { UserAction.SaveMarker, "user_activity.save_marker.title" },
        { UserAction.StreetPreview, "user_activity.street_preview.title" }
    };

    private static readonly Dictionary<UserAction, string[]> Keywords = new Dictionary<UserAction, string[]>
    {
        { UserAction.MyLocation, new [] { "ui.action_button.my_location", "location" } },
        { UserAction.AroundMe, new [] { "ui.action_button.around_me" } },
        { UserAction.AheadOfMe, new [] { "ui.action_button.ahead_of_me" } },
        { UserAction.NearbyMarkers, new [] { "ui.action_button.nearby_markers" } },
        { UserAction.Search, new [] { "preview.search.label" } },
        { UserAction.SaveMarker, new [] { "markers.generic_name" } },
        { UserAction.StreetPreview, new [] { "voice.action.preview" } }
    };

    public static string GetTitle(this UserAction action)
    {
        return Titles[action];
    }

    public static string GetSuggestedInvocationPhrase(this UserAction action)
    {
        return GetTitle(action);
    }

    public static IEnumerable<string> GetKeywords(this UserAction action)
    {
        return Keywords[action];
    }
}

public class NSUserActivity
{
    public string ActivityType { get; private set; }
    public string PersistentIdentifier { get; set; }
    public bool IsEligibleForPublicIndexing { get; set; }
    public bool IsEligibleForSearch { get; set; }
    public string Title { get; set; }
    public IEnumerable<string> Keywords { get; set; }
    public bool IsEligibleForPrediction { get; set; }
    public string SuggestedInvocationPhrase { get; set; }

    public NSUserActivity(UserAction userAction)
    {
        ActivityType = userAction.ToString();
        PersistentIdentifier = ActivityType;
        IsEligibleForPublicIndexing = true;
        IsEligibleForSearch = true;
        Title = userAction.GetTitle();
        Keywords = userAction.GetKeywords();
        IsEligibleForPrediction = true;
        SuggestedInvocationPhrase = userAction.GetSuggestedInvocationPhrase();
    }

    public bool IsUserAction()
    {
        return Enum.TryParse(ActivityType, out UserAction action);
    }
}

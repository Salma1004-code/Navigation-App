public struct PushNotification
{
    public enum ArrivalContext
    {
        Default,
        Launch
    }

    public enum OriginContext
    {
        Remote,
        Local
    }

    public static class Keys
    {
        public static string LocalIdentifier = "GDALocalIdentifier";
        public static string OriginContext = "GDAOriginContext";
        public static string Url = "url";
    }

    public Dictionary<string, object> Payload { get; private set; }
    public string Title { get; private set; }
    public string Subtitle { get; private set; }
    public string Body { get; private set; }
    public string LocalIdentifier { get; private set; }
    public ArrivalContext ArrivalContext { get; private set; }
    public OriginContext OriginContext { get; private set; }
    public UserAction? UserAction { get; private set; }
    public string Url { get; private set; }

    public PushNotification(Dictionary<string, object> payload, ArrivalContext arrivalContext = ArrivalContext.Default)
    {
        Payload = payload;
        Title = null;
        Subtitle = null;
        Body = null;
        LocalIdentifier = null;
        ArrivalContext = arrivalContext;
        OriginContext = OriginContext.Remote;
        UserAction = null;
        Url = null;

        if (payload.TryGetValue("aps", out object apsObj) && apsObj is Dictionary<string, object> aps)
        {
            if (aps.TryGetValue("alert", out object alertObj))
            {
                if (alertObj is Dictionary<string, object> alert)
                {
                    Title = alert["title"] as string;
                    Subtitle = alert["subtitle"] as string;
                    Body = alert["body"] as string;
                }
                else if (alertObj is string alertString)
                {
                    Body = alertString;
                }
            }
        }

        if (payload.TryGetValue(Keys.LocalIdentifier, out object localIdentifierObj))
        {
            LocalIdentifier = localIdentifierObj as string;
        }

        if (payload.TryGetValue(Keys.OriginContext, out object originContextObj) && originContextObj is string originContextString)
        {
            OriginContext = Enum.TryParse(originContextString, out OriginContext originContext) ? originContext : OriginContext.Remote;
        }

        if (payload.TryGetValue(UserActionManager.Keys.UserAction, out object userActionIdentifierObj) && userActionIdentifierObj is string userActionIdentifier)
        {
            UserAction = new UserAction(userActionIdentifier);
        }

        if (payload.TryGetValue(Keys.Url, out object urlObj))
        {
            Url = urlObj as string;
        }
    }
}

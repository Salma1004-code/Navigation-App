public enum RouteChangeReason
{
    Unknown,
    NewDeviceAvailable,
    OldDeviceUnavailable,
    CategoryChange,
    Override,
    WakeFromSleep,
    NoSuitableRouteForCategory,
    RouteConfigurationChange,
    // Add new enum values here as they become available
}

public override string ToString()
{
    return this switch
    {
        RouteChangeReason.Unknown => "unknown",
        RouteChangeReason.NewDeviceAvailable => "new device available",
        RouteChangeReason.OldDeviceUnavailable => "old device unavailable",
        RouteChangeReason.CategoryChange => "category change",
        RouteChangeReason.Override => "override",
        RouteChangeReason.WakeFromSleep => "wake from sleep",
        RouteChangeReason.NoSuitableRouteForCategory => "no suitable route for category",
        RouteChangeReason.RouteConfigurationChange => "route configuration change",
        _ => "unknown - (WARNING) new enum value added"
    };
}

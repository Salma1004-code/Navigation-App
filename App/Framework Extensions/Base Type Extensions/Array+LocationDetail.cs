public static class LocationDetailListExtensions
{
    public static List<IdentifiableLocationDetail> AsIdentifiable(this List<LocationDetail> list)
    {
        return list.Select(ld => new IdentifiableLocationDetail(ld)).ToList();
    }

    public static List<RouteWaypoint> AsRouteWaypoint(this List<LocationDetail> list)
    {
        return list.Select((ld, index) => new RouteWaypoint(index, ld)).ToList();
    }
}

public static class IdentifiableLocationDetailListExtensions
{
    public static List<LocationDetail> AsLocationDetail(this List<IdentifiableLocationDetail> list)
    {
        return list.Select(ild => ild.LocationDetail).ToList();
    }

    public static List<RouteWaypoint> AsRouteWaypoint(this List<IdentifiableLocationDetail> list)
    {
        return list.AsLocationDetail().AsRouteWaypoint();
    }
}

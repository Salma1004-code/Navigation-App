public static class CoordinateRegionExtensions
{
    public static Coordinate NorthWest(this CoordinateRegion region)
    {
        return new Coordinate(region.Center.Latitude + (region.Span.LatitudeDelta / 2.0),
                              region.Center.Longitude - (region.Span.LongitudeDelta / 2.0));
    }

    public static Coordinate SouthEast(this CoordinateRegion region)
    {
        return new Coordinate(region.Center.Latitude - (region.Span.LatitudeDelta / 2.0),
                              region.Center.Longitude + (region.Span.LongitudeDelta / 2.0));
    }
}

public static class PredicateExtensions
{
    public static Predicate CreatePredicate(Coordinate centerCoordinate, double span, string latitudeKey = "latitude", string longitudeKey = "longitude")
    {
        var region = new CoordinateRegion(centerCoordinate, span, span);
        return CreatePredicate(region, latitudeKey, longitudeKey);
    }

    public static Predicate CreatePredicate(CoordinateRegion region, string latitudeKey = "latitude", string longitudeKey = "longitude")
    {
        var format = $"{latitudeKey} < %f AND {latitudeKey} > %f AND {longitudeKey} > %f AND {longitudeKey} < %f";
        var topLeft = region.NorthWest();
        var bottomRight = region.SouthEast();

        return new Predicate(format, topLeft.Latitude, bottomRight.Latitude, topLeft.Longitude, bottomRight.Longitude);
    }
}

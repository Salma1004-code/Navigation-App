public static class POIListExtensions
{
    public static Dictionary<CompassDirection, List<POI>> Quadrants(this List<POI> list, List<CompassDirection> includedQuadrants, CLLocation location, CLLocationDirection heading, List<SuperCategory> categories, int maxLengthPerQuadrant)
    {
        // Assuming you have a SpatialDataView class defined somewhere
        var quadrants = SpatialDataView.GetQuadrants(heading);
        var sortPredicate = new SortDistance(location);
        var categoryFilterPredicate = new FilterSuperCategories(categories);

        var queues = new Dictionary<CompassDirection, POIQueue>();
        foreach (var direction in includedQuadrants)
        {
            queues[direction] = new POIQueue(maxLengthPerQuadrant + 10, sortPredicate, categoryFilterPredicate);
        }

        foreach (var poi in list)
        {
            var dir = CompassDirection.From(poi.BearingToClosestLocation(location), quadrants);
            queues[dir]?.Insert(poi);
        }

        var locationFilterPredicate = new FilterLocation(location).Invert();

        var results = new Dictionary<CompassDirection, List<POI>>();
        foreach (var kvp in queues)
        {
            results[kvp.Key] = kvp.Value.POIs.FilteredBy(locationFilterPredicate, maxLengthPerQuadrant);
        }

        return results;
    }

    // You would need to implement the Sorted, Filtered, and other methods used in this extension
}

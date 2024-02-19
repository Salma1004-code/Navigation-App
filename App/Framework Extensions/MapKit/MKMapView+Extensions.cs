public static class MapViewExtensions
{
    public static void RemoveAllAnnotations(this MapView mapView)
    {
        if (mapView.Annotations.Count > 0)
        {
            mapView.RemoveAnnotations(mapView.Annotations);
        }
    }

    private static void ShowAnnotationAndCenter(this MapView mapView, Annotation annotation)
    {
        if (mapView.CenterCoordinate != annotation.Coordinate)
        {
            mapView.ShowAnnotations(new List<Annotation> { annotation }, true);
            mapView.CenterCoordinate = annotation.Coordinate;
        }
    }

    public static void ConfigureEmptyMapView(this MapView mapView)
    {
        mapView.RemoveAllAnnotations();
        mapView.UserTrackingMode = UserTrackingMode.Follow;
    }

    // You would need to implement the Configure, ConfigureMap, and other methods used in this extension
}

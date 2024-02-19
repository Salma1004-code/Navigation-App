public static class MapViewDelegateExtensions
{
    public static AnnotationView ViewForLocationDetailAnnotation(this IMapViewDelegate mapViewDelegate, MapView mapView, LocationDetailAnnotation annotation)
    {
        var identifier = LocationDetailAnnotationView.Identifier;
        AnnotationView view;

        var dequeuedView = mapView.DequeueReusableAnnotationView(identifier);
        if (dequeuedView != null)
        {
            dequeuedView.Annotation = annotation;
            view = dequeuedView;
        }
        else
        {
            view = new LocationDetailAnnotationView(annotation, identifier);
        }

        return view;
    }

    // You would need to implement the ViewForWaypointDetailAnnotation and ViewForClusterAnnotation methods used in this extension
}

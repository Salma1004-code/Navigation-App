using System;
using System.Collections.Generic;
using iOS_GPX_Framework; // Assuming this is the namespace where GPX-related classes are defined
using CoreMotion; // Assuming this is the namespace where CMMotionActivity is defined

public delegate void GPXActivity(string activity);

public struct GPXLocation
{
    public CLLocation Location { get; set; }
    public double? DeviceHeading { get; set; }
    public GPXActivity? Activity { get; set; }
}

public static class GPXBoundsExtensions
{
    public static GPXBounds CreateWithLocations(List<GPXLocation> locations)
    {
        if (locations.Count == 0 || locations[0].Location == null)
            return null;

        double minLatitude = locations[0].Location.Coordinate.Latitude;
        double maxLatitude = locations[0].Location.Coordinate.Latitude;
        double minLongitude = locations[0].Location.Coordinate.Longitude;
        double maxLongitude = locations[0].Location.Coordinate.Longitude;

        foreach (var gpxLocation in locations)
        {
            var location = gpxLocation.Location;

            if (location.Coordinate.Latitude < minLatitude)
                minLatitude = location.Coordinate.Latitude;
            if (location.Coordinate.Latitude > maxLatitude)
                maxLatitude = location.Coordinate.Latitude;
            if (location.Coordinate.Longitude < minLongitude)
                minLongitude = location.Coordinate.Longitude;
            if (location.Coordinate.Longitude > maxLongitude)
                maxLongitude = location.Coordinate.Longitude;
        }

        return new GPXBounds(minLatitude, minLongitude, maxLatitude, maxLongitude);
    }
}

public static class GPXRootExtensions
{
    public static GPXRoot DefaultRoot()
    {
        var creator = $"{AppContext.AppDisplayName} {AppContext.AppVersion} ({AppContext.AppBuild})";
        var root = new GPXRoot(creator);

        var metadata = new GPXMetadata();
        metadata.Time = DateTime.Now;
        metadata.Description = $"Created on {UIDevice.CurrentDevice.Model} ({UIDevice.CurrentDevice.SystemName} {UIDevice.CurrentDevice.SystemVersion})";

        var author = new GPXAuthor();
        author.Name = UIDevice.CurrentDevice.Name;
        metadata.Author = author;

        root.Metadata = metadata;

        return root;
    }

    public static GPXRoot CreateGPXWithTrackLocations(List<GPXLocation> trackLocations)
    {
        var root = DefaultRoot();
        root.Metadata?.Bounds = GPXBoundsExtensions.CreateWithLocations(trackLocations);

        var trackSegment = new GPXTrackSegment();
        foreach (var gpxLocation in trackLocations)
        {
            trackSegment.AddTrackpoint(new GPXTrackPoint(gpxLocation));
        }

        var track = new GPXTrack();
        track.AddTracksegment(trackSegment);

        root.AddTrack(track);

        return root;
    }
}

public static class GPXWaypointExtensions
{
    public static DateTime NoDateIdentifier()
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }

    public static bool HasSoundscapeExtension(this GPXWaypoint waypoint)
    {
        return waypoint.Extensions?.SoundscapeExtensions != null;
    }

    public static GPXWaypoint WithGPXLocation(this GPXWaypoint waypoint, GPXLocation gpxLocation)
    {
        var location = gpxLocation.Location;

        waypoint.Latitude = location.Coordinate.Latitude;
        waypoint.Longitude = location.Coordinate.Longitude;
        waypoint.Elevation = location.Altitude;
        waypoint.Time = location.Timestamp;

        var garminExtension = new GPXTrackPointExtensions();
        garminExtension.Speed = Convert.ToDecimal(location.Speed);
        garminExtension.Course = Convert.ToDecimal(location.Course);

        var soundscapeExtension = new GPXSoundscapeExtensions();
        soundscapeExtension.HorizontalAccuracy = Convert.ToDecimal(location.HorizontalAccuracy);
        soundscapeExtension.VerticalAccuracy = Convert.ToDecimal(location.VerticalAccuracy);

        if (gpxLocation.DeviceHeading.HasValue)
        {
            soundscapeExtension.DeviceHeading = Convert.ToDecimal(gpxLocation.DeviceHeading.Value);
        }

        if (gpxLocation.Activity != null && gpxLocation.Activity != ActivityType.Unknown.ToString())
        {
            soundscapeExtension.Activity = gpxLocation.Activity.Invoke();
        }

        var extensions = new GPXExtensions();
        extensions.GarminExtensions = garminExtension;
        extensions.SoundscapeExtensions = soundscapeExtension;

        waypoint.Extensions = extensions;

        return waypoint;
    }

    public static GPXLocation ToGPXLocation(this GPXWaypoint waypoint)
    {
        double speed = -1;
        double course = -1;
        double horizontalAccuracy = -1;
        double verticalAccuracy = -1;
        double trueHeading = -1;
        double magneticHeading = -1;
        double headingAccuracy = -1;
        double? deviceHeading = null;
        GPXActivity? activity = null;

        if (waypoint.Extensions != null && waypoint.Extensions.SoundscapeExtensions != null)
        {
            speed = Convert.ToDouble(waypoint.Extensions.SoundscapeExtensions.Speed);
            course = Convert.ToDouble(waypoint.Extensions.SoundscapeExtensions.Course);
            horizontalAccuracy = Convert.ToDouble(waypoint.Extensions.SoundscapeExtensions.HorizontalAccuracy);
            verticalAccuracy = Convert.ToDouble(waypoint.Extensions.SoundscapeExtensions.VerticalAccuracy);
            trueHeading = Convert.ToDouble(waypoint.Extensions.SoundscapeExtensions.TrueHeading);
            magneticHeading = Convert.ToDouble(waypoint.Extensions.SoundscapeExtensions.MagneticHeading);
            headingAccuracy = Convert.ToDouble(waypoint.Extensions.SoundscapeExtensions.HeadingAccuracy);
            deviceHeading = Convert.ToDouble(waypoint.Extensions.SoundscapeExtensions.DeviceHeading);

            if (!string.IsNullOrEmpty(waypoint.Extensions.SoundscapeExtensions.Activity) && waypoint.Extensions.SoundscapeExtensions.Activity != ActivityType.Unknown.ToString())
            {
                activity = () => waypoint.Extensions.SoundscapeExtensions.Activity;
            }
        }

        var location = new CLLocation(
            new CLLocationCoordinate2D(waypoint.Latitude, waypoint.Longitude),
            waypoint.Elevation,
            horizontalAccuracy,
            verticalAccuracy,
            course,
            speed,
            waypoint.Time ?? NoDateIdentifier()
        );

        return new GPXLocation
        {
            Location = location,
            DeviceHeading = deviceHeading,
            Activity = activity
        };
    }
}

public static class CLLocationCoordinate2DExtensions
{
    public static GPXRoute ToGPXRoute(this List<CLLocationCoordinate2D> coordinates)
    {
        var routePoints = coordinates.ConvertAll(coord => GPXRoutePoint.RoutepointWithLatitudeLongitude(coord.Latitude, coord.Longitude));
        var route = new GPXRoute();
        route.AddRoutepoints(routePoints.ToArray());

        return route;
    }
}

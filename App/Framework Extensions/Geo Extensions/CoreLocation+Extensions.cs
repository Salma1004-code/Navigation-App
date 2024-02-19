using CoreLocation;
using System;
using System.Collections.Generic;

public static class CLLocationExtensions
{
    public static CLLocation WithTimestamp(this CLLocation location, DateTime timestamp)
    {
        return new CLLocation(
            location.Coordinate.Latitude,
            location.Coordinate.Longitude,
            location.Altitude,
            location.HorizontalAccuracy,
            location.VerticalAccuracy,
            location.Course,
            location.Speed,
            timestamp);
    }

    public static CLLocation WithSpeed(this CLLocation location, double speed)
    {
        return new CLLocation(
            location.Coordinate.Latitude,
            location.Coordinate.Longitude,
            location.Altitude,
            location.HorizontalAccuracy,
            location.VerticalAccuracy,
            location.Course,
            speed,
            location.Timestamp);
    }

    public static CLLocation WithCourse(this CLLocation location, double course)
    {
        return new CLLocation(
            location.Coordinate.Latitude,
            location.Coordinate.Longitude,
            location.Altitude,
            location.HorizontalAccuracy,
            location.VerticalAccuracy,
            course,
            location.Speed,
            location.Timestamp);
    }

    public static double BearingTo(this CLLocation location, CLLocation destination)
    {
        return location.Coordinate.BearingTo(destination.Coordinate);
    }

    public static bool IsTravelingTowards(this CLLocation location, CLLocationCoordinate2D coordinate, double angularWindowRange)
    {
        if (location.Course < 0 || angularWindowRange < 0)
            return false;

        double bearingToCoordinate = location.Coordinate.BearingTo(coordinate);
        if (!DirectionRange.TryCreate(bearingToCoordinate, angularWindowRange, out DirectionRange directionRange))
            return false;

        return directionRange.Contains(location.Course);
    }
}

public static class CLLocationArrayExtensions
{
    public static List<CLLocation> TransformToAverageWalkingSpeed(this List<CLLocation> locations)
    {
        return locations.Transform(CLLocationSpeed.AverageWalkingSpeed);
    }

    public static List<CLLocation> Transform(this List<CLLocation> locations, double speed)
    {
        if (speed <= 0 || locations.Count <= 1)
            return locations;

        var transformedLocations = new List<CLLocation>();
        for (int i = 0; i < locations.Count; i++)
        {
            var location = locations[i];
            if (i == 0)
            {
                transformedLocations.Add(location.WithSpeed(speed));
                continue;
            }

            var prevLocation = transformedLocations[i - 1];
            var distance = location.DistanceFrom(prevLocation);
            CLLocation updatedLocation;
            if (distance > 0)
            {
                var interval = distance / speed;
                var updatedTimestamp = prevLocation.Timestamp.AddSeconds(interval);
                updatedLocation = location.WithTimestamp(updatedTimestamp).WithSpeed(speed);
            }
            else
            {
                var updatedTimestamp = prevLocation.Timestamp.AddSeconds(1);
                updatedLocation = location.WithTimestamp(updatedTimestamp).WithSpeed(0);
            }
            transformedLocations.Add(updatedLocation);
        }
        return transformedLocations;
    }
}

public static class CLLocationCoordinate2DExtensions
{
    public static bool IsValidLocationCoordinate(this CLLocationCoordinate2D coordinate)
    {
        return CLLocationCoordinate2DIsValid(coordinate) && coordinate.Latitude != 0 && coordinate.Longitude != 0;
    }

    public static double DistanceFrom(this CLLocationCoordinate2D coordinate, CLLocationCoordinate2D otherCoordinate)
    {
        return new CLLocation(coordinate.Latitude, coordinate.Longitude)
            .DistanceFrom(new CLLocation(otherCoordinate.Latitude, otherCoordinate.Longitude));
    }

    public static double BearingTo(this CLLocationCoordinate2D coordinate, CLLocationCoordinate2D destinationCoordinate)
    {
        return coordinate.GetBearingTo(destinationCoordinate);
    }

    public static CLLocationCoordinate2D Destination(this CLLocationCoordinate2D coordinate, double distance, double bearing)
    {
        return coordinate.GetDestination(distance, bearing);
    }

    public static CLLocationCoordinate2D CoordinateBetween(this CLLocationCoordinate2D coordinate, CLLocationCoordinate2D otherCoordinate, double distance)
    {
        return coordinate.GetCoordinateBetween(otherCoordinate, distance);
    }
}

public static class CLLocationDirectionExtensions
{
    public const double North = 0.0;
    public const double East = 90.0;
    public const double South = 180.0;
    public const double West = 270.0;

    public const double Ahead = 0.0;
    public const double Right = 90.0;
    public const double Behind = 180.0;
    public const double Left = 270.0;

    public static bool IsValid(this double direction)
    {
        return direction >= 0;
    }

    public static double BearingTo(this double direction, double destinationDirection)
    {
        return destinationDirection + 360 - direction;
    }

    public static double AddDegrees(this double direction, double degrees)
    {
        return (direction + degrees + 360.0) % 360.0;
    }
}
   public static class CLLocationSpeedExtensions
   {
       public const double AverageWalkingSpeed = 1.4; // meters per second
   }
public static class DoubleExtensions
{
    public static double ToRadians(this double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    public static double ToDegrees(this double radians)
    {
        return radians * 180.0 / Math.PI;
    }

    public static string FormatAsDegrees(this double degrees)
    {
        return $"{degrees:F2}°";
    }
}

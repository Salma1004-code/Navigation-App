using System;
using CoreLocation;

public static class KalmanFilterExtensions
{
    public static CLLocation Process(this KalmanFilter filter, CLLocation location)
    {
        var vector = new double[] { location.Coordinate.Latitude, location.Coordinate.Longitude };
        var timestamp = location.Timestamp;
        var accuracy = location.HorizontalAccuracy;

        var filteredVector = filter.Process(vector, timestamp, accuracy);
        if (filteredVector == null || filteredVector.Length != 2)
        {
            return location;
        }

        return new CLLocation(
            new CLLocationCoordinate2D(filteredVector[0], filteredVector[1]),
            location.Altitude,
            location.HorizontalAccuracy,
            location.VerticalAccuracy,
            location.Course,
            location.Speed,
            location.Timestamp);
    }
}

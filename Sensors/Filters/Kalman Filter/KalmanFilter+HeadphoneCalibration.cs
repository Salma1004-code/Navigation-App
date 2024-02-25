using System;

public static class KalmanFilterExtensions
{
    public static HeadphoneCalibration Process(this KalmanFilter filter, HeadphoneCalibration calibration)
    {
        var vector = new double[] { Math.Sin(calibration.ValueInRadians), Math.Cos(calibration.ValueInRadians) };
        var timestamp = calibration.Timestamp;
        var accuracy = calibration.Accuracy;

        var filteredVector = filter.Process(vector, timestamp, accuracy);
        if (filteredVector == null || filteredVector.Length != 2)
        {
            return calibration;
        }

        var filteredValueInRadians = Math.Atan2(filteredVector[0], filteredVector[1]);
        var filteredValue = CircularQuantity.FromRadians(filteredValueInRadians).Normalized();

        return new HeadphoneCalibration(filteredValue, calibration.Accuracy, calibration.Timestamp);
    }
}

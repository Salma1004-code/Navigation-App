using System;
using System.Linq;

public class KalmanFilter
{
    private const double MinimumAccuracy = 0.1;
    private readonly double sigma;
    private double covariance = 0.0;
    private double[] estimate;
    private DateTime? timestamp;

    public KalmanFilter(double sigma)
    {
        this.sigma = sigma;
    }

    public double[] Process(double[] newVector, DateTime newTimestamp, double newAccuracy)
    {
        double accuracy = Math.Max(newAccuracy, MinimumAccuracy);
        double measurementVariance = accuracy * accuracy;

        if (estimate == null || timestamp == null)
        {
            estimate = newVector;
            timestamp = newTimestamp;
            covariance = measurementVariance;
            return newVector;
        }

        if (estimate.Length != newVector.Length)
        {
            return null;
        }

        double interval = (newTimestamp - timestamp.Value).TotalSeconds;
        if (interval > 0)
        {
            covariance += interval * sigma * sigma;
        }

        double kalmanGain = covariance / (covariance + measurementVariance);
        double[] filteredVector = estimate.Zip(newVector, (e, nv) => e + kalmanGain * (nv - e)).ToArray();

        estimate = filteredVector;
        timestamp = newTimestamp;
        covariance = (1 - kalmanGain) * covariance;

        return filteredVector;
    }

    public void Reset()
    {
        covariance = 0.0;
        estimate = null;
        timestamp = null;
    }
}

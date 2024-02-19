public static class CircularQuantityListExtensions
{
    private static List<double> Radians(this List<CircularQuantity> list)
    {
        return list.Select(q => q.Normalized().ValueInRadians).ToList();
    }

    private static double? XMean(this List<CircularQuantity> list)
    {
        if (list.Count == 0) return null;
        return list.Radians().Sum(r => Math.Cos(r)) / list.Count;
    }

    private static double? YMean(this List<CircularQuantity> list)
    {
        if (list.Count == 0) return null;
        return list.Radians().Sum(r => Math.Sin(r)) / list.Count;
    }

    public static CircularQuantity? Mean(this List<CircularQuantity> list)
    {
        if (list.Count == 0) return null;
        var xMean = list.XMean();
        var yMean = list.YMean();
        if (xMean == null || yMean == null) return null;
        var meanInRadians = Math.Atan2(yMean.Value, xMean.Value);
        return new CircularQuantity(meanInRadians).Normalized();
    }

    public static double? MeanInDegrees(this List<CircularQuantity> list)
    {
        return list.Mean()?.ValueInDegrees;
    }

    public static double? MeanInRadians(this List<CircularQuantity> list)
    {
        return list.Mean()?.ValueInRadians;
    }

    public static CircularQuantity? Stdev(this List<CircularQuantity> list)
    {
        if (list.Count == 0) return null;
        var xMean = list.XMean();
        var yMean = list.YMean();
        if (xMean == null || yMean == null) return null;
        var stdevInRadians = Math.Sqrt(-2 * Math.Log(Math.Sqrt((yMean.Value * yMean.Value) + (xMean.Value * xMean.Value))));
        return new CircularQuantity(stdevInRadians).Normalized();
    }

    public static double? StdevInDegrees(this List<CircularQuantity> list)
    {
        return list.Stdev()?.ValueInDegrees;
    }

    public static double? StdevInRadians(this List<CircularQuantity> list)
    {
        return list.Stdev()?.ValueInRadians;
    }
}

public static class FloatingPointListExtensions
{
    public static T? Sum<T>(this List<T> list) where T : struct
    {
        if (list.Count == 0) return null;
        return list.Sum();
    }

    public static T? Mean<T>(this List<T> list) where T : struct
    {
        if (list.Count == 0) return null;
        var sum = list.Sum();
        if (sum == null) return null;
        return (T)(sum.Value / list.Count);
    }

    public static T? Stdev<T>(this List<T> list) where T : struct
    {
        if (list.Count == 0) return null;
        var mean = list.Mean();
        if (mean == null) return null;
        var variance = list.Sum(x => Math.Pow((double)(x - mean.Value), 2));
        return (T)Math.Sqrt((double)(variance / list.Count));
    }
}

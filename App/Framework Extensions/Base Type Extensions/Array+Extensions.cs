public static class HashableListExtensions
{
    public static HashSet<T> ToSet<T>(this List<T> list) where T : IEquatable<T>
    {
        return new HashSet<T>(list);
    }

    public static List<T> DropDuplicates<T>(this List<T> list) where T : IEquatable<T>
    {
        return list.ToSet().ToList();
    }
}

public static class ReferenceEntityListExtensions
{
    // Assuming you have a ReferenceEntity class defined somewhere
    // You would need to implement the methods used in this extension
    // such as distanceToClosestLocation, getPOI, etc.
}

public static class AnyCancellableListExtensions
{
    public static void CancelAndRemoveAll(this List<AnyCancellable> list)
    {
        foreach (var cancellable in list)
        {
            cancellable.Cancel();
        }
        list.Clear();
    }
}

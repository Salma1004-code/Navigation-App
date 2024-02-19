public static class ThreadExtensions
{
    public static string ThreadName(this Thread thread)
    {
        // In C#, the Name property of a Thread can be used directly
        return thread.Name ?? thread.ManagedThreadId.ToString();
    }
}

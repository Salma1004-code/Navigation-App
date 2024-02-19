public static class DataExtensions
{
    public static byte[] From<T>(T value) where T : struct
    {
        int size = Marshal.SizeOf(value);
        byte[] arr = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(value, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);
        return arr;
    }

    public static T? To<T>(this byte[] data) where T : struct
    {
        if (data.Length < Marshal.SizeOf<T>()) return null;
        GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        T? result = (T?)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
        handle.Free();
        return result;
    }
}

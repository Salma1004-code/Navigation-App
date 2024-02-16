using System;

public enum UniversalLinkVersion
{
    // Define enum members representing different versions in the universal link URL
    v1,
    v2,
    v3

    // Define a static method to determine the current version based on the path
    public static UniversalLinkVersion CurrentVersionForPath(UniversalLinkPath path)
    {
        switch (path)
        {
            case UniversalLinkPath.Experience:
                return v3;
            case UniversalLinkPath.ShareMarker:
                return v1;
            default:
                throw new ArgumentException("Unknown UniversalLinkPath value", nameof(path));
        }
    }

    // Define a static constant representing the default version
    public static UniversalLinkVersion DefaultVersion => v1;
}

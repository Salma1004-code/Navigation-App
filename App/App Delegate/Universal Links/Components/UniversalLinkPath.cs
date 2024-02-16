using System;

// Define an enum called UniversalLinkPath
public enum UniversalLinkPath
{
    // Define enum members representing different paths in the universal link URL
    Experience,
    ShareMarker
}

public static class UniversalLinkPathExtensions
{
    // Define an extension method to get the string representation of the enum values
    public static string ToRawValue(this UniversalLinkPath path)
    {
        // Convert enum values to strings and return
        switch (path)
        {
            case UniversalLinkPath.Experience:
                return "experiences";
            case UniversalLinkPath.ShareMarker:
                return "sharemarker";
            default:
                throw new ArgumentOutOfRangeException(nameof(path), path, "Unknown UniversalLinkPath value");
        }
    }
}

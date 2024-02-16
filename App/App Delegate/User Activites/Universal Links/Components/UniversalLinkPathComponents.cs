using System;

public class UniversalLinkPathComponents
{
    // Define properties to hold the path and version components
    public UniversalLinkPath Path { get; }
    public UniversalLinkVersion Version { get; }

    // Define a property to generate the versioned path
    public string VersionedPath => $"/{Version.ToRawValue()}/{Path.ToRawValue()}";

    // Initialize UniversalLinkPathComponents from a given path string
    public UniversalLinkPathComponents(string path)
    {
        // Split the path string into components based on '/'
        string[] pathComponents = path.Split(new[] { '/' }, 2);

        if (pathComponents.Length == 1)
        {
            // URL path does not include a version
            string pRawValue = pathComponents[0];

            // Parse the path component
            if (!Enum.TryParse(pRawValue, out UniversalLinkPath pathEnum))
                throw new ArgumentException("Invalid path component", nameof(path));

            // Use the default version
            Path = pathEnum;
            Version = UniversalLinkVersion.DefaultVersion;
        }
        else if (pathComponents.Length == 2)
        {
            // URL path includes a version
            string vRawValue = pathComponents[0];
            string pRawValue = pathComponents[1];

            // Parse the version and path components
            if (!Enum.TryParse(vRawValue, out UniversalLinkVersion versionEnum))
                throw new ArgumentException("Invalid version component", nameof(path));
            if (!Enum.TryParse(pRawValue, out UniversalLinkPath pathEnum))
                throw new ArgumentException("Invalid path component", nameof(path));

            Version = versionEnum;
            Path = pathEnum;
        }
        else
        {
            // Failed to parse components
            throw new ArgumentException("Invalid path format", nameof(path));
        }
    }

    // Initialize UniversalLinkPathComponents with given path and default version
    public UniversalLinkPathComponents(UniversalLinkPath path)
    {
        Path = path;
        // Determine the current version for the given path
        Version = UniversalLinkVersion.CurrentVersionForPath(path);
    }
}

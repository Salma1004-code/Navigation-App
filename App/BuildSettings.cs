using System;
using System.Linq;

public static class BuildSettings
{
    public enum Configuration
    {
        Debug,
        Adhoc,
        Release
    }

    public enum Source
    {
        Local,
        AppCenter,
        TestFlight,
        AppStore
    }

    public static Configuration configuration
    {
        get
        {
            #if DEBUG
            return Configuration.Debug;
            #elif ADHOC
            return Configuration.Adhoc;
            #else
            return Configuration.Release;
            #endif
        }
    }

    public static Source source
    {
        get
        {
            switch (configuration)
            {
                case Configuration.Debug:
                    return Source.Local;
                case Configuration.Adhoc:
                    return Source.AppCenter;
                case Configuration.Release:
                    // You need to implement your own logic here to determine the source
                    // based on the presence of a specific App Store receipt file
                    // For example, you might check a specific file path or use an API
                    if (AppStoreReceiptExists())
                    {
                        return Source.TestFlight;
                    }
                    else
                    {
                        return Source.AppStore;
                    }
                default:
                    throw new Exception("Invalid configuration");
            }
        }
    }

    public static bool isTesting
    {
        get
        {
            // Check if the app is currently being tested
            return Environment.GetCommandLineArgs().Contains("-TESTING");
        }
    }

    // This is a placeholder for your own implementation
    private static bool AppStoreReceiptExists()
    {
        // Check if the App Store receipt file exists
        // You might check a specific file path or use an API
        return false;
    }
}

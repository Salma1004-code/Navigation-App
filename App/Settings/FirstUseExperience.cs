using System;

public class FirstUseExperience
{
    public enum Experience
    {
        Oobe,
        BeaconTutorial,
        MarkerTutorial,
        PreviewTutorial,
        RouteTutorial,
        ICloudBackup,
        PreviewRoadFinder,
        PreviewRoadFinderError,
        AddDevice,
        DeviceReachabilityAlert,
        Share,
        DonateSiriShortcuts,
        OobeSelectBeacon
        // You may need to add more experiences based on your needs
    }

    public static bool DidComplete(Experience experience)
    {
        // If there is no value for the key, `false` is returned
        // You need to replace `UserDefaults.Standard.BoolForKey` with your own implementation
        return UserDefaults.Standard.BoolForKey(experience.ToString());
    }

    public static void SetDidComplete(bool didComplete = true, Experience experience)
    {
        // You need to replace `UserDefaults.Standard.SetValueForKey` with your own implementation
        UserDefaults.Standard.SetValueForKey(didComplete, experience.ToString());
    }
}

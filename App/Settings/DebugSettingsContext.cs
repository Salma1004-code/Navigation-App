using System;
using Foundation;
using AVFoundation;
using MapKit;

public static class NotificationNames
{
    public static readonly NSString IsHeadphoneMotionEnabledDidChange = new NSString("GDAIsHeadphoneMotionEnabledDidChange");
}

public class DebugSettingsContext
{
    // Keys
    private static class UserDefaultKeys
    {
        public const string ShowLogConsole = "GDAShowLogConsole";
        public const string Loggers = "GDALoggers";
        public const string Theme = "Theme";
        public const string LocationAccuracy = "GDALocationAccuracy";
        public const string IsUsingHeading = "GDAUsingHeading";

        public const string EnvRenderingAlgorithm = "GDA3DRenderingAlgorithm";
        public const string EnvRenderingDistance = "GDA3DRenderingDistance";
        public const string EnvRenderingReverbEnable = "GDA3DRenderingReverbEnable";
        public const string EnvRenderingReverbPreset = "GDA3DRenderingReverbPreset";
        public const string EnvRenderingReverbBlend = "GDA3DRenderingReverbBlend";
        public const string EnvRenderingReverbLevel = "GDA3DRenderingReverbLevel";
        public const string EnvReverbFilterActive = "GDA3DReverbFilterActive";
        public const string EnvReverbFilterBandwidth = "GDA3DReverbFilterBandwidth";
        public const string EnvReverbFilterBypass = "GDA3DReverbFilterBypass";
        public const string EnvReverbFilterType = "GDA3DReverbFilterType";
        public const string EnvReverbFilterFrequency = "GDA3DReverbFilterFrequency";
        public const string EnvReverbFilterGain = "GDA3DReverbFilterGain";

        public const string MapType = "GDAMapType";
        public const string MapFilterPlaces = "GDAMapFilterPlaces";
        public const string MapFilterLandmarks = "GDAMapFilterLandmarks";
        public const string MapFilterMobility = "GDAMapFilterMobility";
        public const string MapFilterInformation = "GDAMapFilterInformation";
        public const string MapFilterSafety = "GDAMapFilterSafety";
        public const string MapFilterObjects = "GDAMapFilterObjects";
        public const string MapFilterIntersections = "GDAMapFilterIntersections";
        public const string MapFilterFootprints = "GDAMapFilterFootprints";

        public const string GPXSimulationAudioEnabled = "GDASimulationAudioEnabled";
        public const string GPXSimulationAudioPauseWithSimulation = "GDASimulationAudioPauseWithSimulation";
        public const string GPXSimulationAudioVolume = "GDASimulationAudioVolume";

        public const string ServicesHostName = "GDAServicesHostName";
        public const string AssetsHostName = "GDAAssetsHostName";

        public const string CacheDuration = "GDACacheDuration";

        public const string IsHeadphoneMotionEnabled = "GDAIsHeadphoneMotionEnabled";
        public const string IsHeadphoneMotionVerboseLoggingEnabled = "GDAIsHeadphoneMotionVerboseLoggingEnabled";
        public const string IsHeadphoneMotionDeviceHeadingEnabled = "GDAIsHeadphoneMotionDeviceHeadingEnabled";
        public const string IsHeadphoneMotionCourseEnabled = "GDAIsHeadphoneMotionCourseEnabled";

        public const string DoubleLocalizedStrings = "NSDoubleLocalizedStrings";
        public const string BoundedPseudolanguage = "NSSurroundLocalizedStrings";

        public const string LocalPushNotification1TimeInternal = "GDALocalPushNotification1TimeInternal";
        public const string LocalPushNotification2TimeInternal = "GDALocalPushNotification2TimeInternal";

        public const string PresentSurveyAlert = "GDAPresentSurveyAlert";
    }

    // Shared Instance
    public static DebugSettingsContext Shared { get; } = new DebugSettingsContext();

    // User Defaults
    private NSUserDefaults UserDefaults => NSUserDefaults.StandardUserDefaults;

    // Initialization
    private DebugSettingsContext()
    {
        UserDefaults.RegisterDefaults(new NSDictionary(
            UserDefaultKeys.MapType, (int)MKMapType.Standard,
            UserDefaultKeys.MapFilterPlaces, true,
            UserDefaultKeys.MapFilterLandmarks, true,
            UserDefaultKeys.MapFilterMobility, true,
            UserDefaultKeys.MapFilterInformation, true,
            UserDefaultKeys.MapFilterSafety, true,
            UserDefaultKeys.MapFilterObjects, true,
            UserDefaultKeys.MapFilterIntersections, false,
            UserDefaultKeys.MapFilterFootprints, true,
            UserDefaultKeys.IsUsingHeading, true,
            UserDefaultKeys.CacheDuration, (double)(60 * 60), // In debug mode, default to 1 hour for the cache time interval
            UserDefaultKeys.EnvRenderingAlgorithm, -1,
            UserDefaultKeys.EnvRenderingDistance, 1.0,
            UserDefaultKeys.EnvRenderingReverbEnable, true,
            UserDefaultKeys.EnvRenderingReverbPreset, (int)AVAudioUnitReverbPreset.MediumRoom,
            UserDefaultKeys.EnvRenderingReverbBlend, 0.10f,
            UserDefaultKeys.EnvRenderingReverbLevel, -20.0f,
            UserDefaultKeys.EnvReverbFilterActive, false,
            UserDefaultKeys.EnvReverbFilterBandwidth, 0.5f,
            UserDefaultKeys.IsHeadphoneMotionEnabled, false,
            UserDefaultKeys.IsHeadphoneMotionVerboseLoggingEnabled, false,
            UserDefaultKeys.IsHeadphoneMotionDeviceHeadingEnabled, true,
            UserDefaultKeys.LocalPushNotification1TimeInternal, 172800, // 2 days
            UserDefaultKeys.LocalPushNotification2TimeInternal, 345600, // 4 days
            UserDefaultKeys.PresentSurveyAlert, false
        ));
    }

    // Properties
    public bool ShowLogConsole
    {
        get => UserDefaults.BoolForKey(UserDefaultKeys.ShowLogConsole);
        set => UserDefaults.SetBool(value, UserDefaultKeys.ShowLogConsole);
    }

    public Logger[] Loggers
    {
        get
        {
            var rawValues = UserDefaults.ArrayForKey(UserDefaultKeys.Loggers) as NSNumber[];
            if (rawValues == null)
                return new[] { Logger.Local }; // Default loggers
            
            return Logger.FromRawValues(rawValues);
        }
        set => UserDefaults.SetObject(Logger.ToRawValues(value), UserDefaultKeys.Loggers);
    }

    public string ServicesHostName
    {
        get => UserDefaults.StringForKey(UserDefaultKeys.ServicesHostName);
        set => UserDefaults.SetString(value, UserDefaultKeys.ServicesHostName);
    }

    public string AssetsHostName
    {
        get => UserDefaults.StringForKey(UserDefaultKeys.AssetsHostName);
        set => UserDefaults.SetString(value, UserDefaultKeys.AssetsHostName);
    }

    public bool GPXSimulationAudioEnabled
    {
        get => UserDefaults.BoolForKey(UserDefaultKeys.GPXSimulationAudioEnabled);
        set => UserDefaults.SetBool(value, UserDefaultKeys.GPXSimulationAudioEnabled);
    }

    public bool GPXSimulationAudioPauseWithSimulation
    {
        get => UserDefaults.BoolForKey(UserDefaultKeys.GPXSimulationAudioPauseWithSimulation);
        set => UserDefaults.SetBool(value, UserDefaultKeys.GPXSimulationAudioPauseWithSimulation);
    }

    public float GPXSimulationAudioVolume
    {
        get => UserDefaults.FloatForKey(UserDefaultKeys.GPXSimulationAudioVolume);
        set

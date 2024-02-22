using System;

public enum StaticAudioEngineAsset
{
    // Earcons/Glyphs
    EnterMode,
    ExitMode,
    Hush,
    InfoAlert,
    InvalidFunction,
    StartJourney,
    StopJourney,
    LocationSense,
    MobilitySense,
    PoiSense,
    SafetySense,
    AppLaunch,
    FlagFound,
    HuntComplete,
    Offline,
    Online,
    CalibrationSuccess,
    LowConfidence, // This isn't currently used
    ConnectionSuccess,
    BeaconFound,
    TourPoiSense,

    // Constants
    StartListening = EnterMode,
    StopListening = ExitMode,
    TellMeMore = AppLaunch,

    // Continuous Audio
    CalibrationInProgress,

    // Location Preview
    PreviewStart,
    PreviewEnd,
    StreetFound,
    TravelStart,
    TravelInter,
    TravelEnd,
    TravelReverse,
    RoadFinderError
}

public static class StaticAudioEngineAssetExtensions
{
    public static string GetDescription(this StaticAudioEngineAsset asset)
    {
        return asset.ToString();
    }
}

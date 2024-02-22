using System;

public enum DynamicAudioEngineAssetSelector
{
    Far,
    Near
}

public interface IDynamicAudioEngineAsset
{
    string GetDescription();
    DynamicAudioEngineAssetSelector GetSelector(AssetInput input);
}

public class AssetInput
{
    public Location User { get; set; }
    public Location Beacon { get; set; }
}

public class Location
{
    // Add properties as needed
    public double Distance(Location other)
    {
        // Replace this with your actual distance calculation logic
        return 0.0;
    }
}

public enum ClassicBeacon
{
    BeatOn,
    BeatOff
}

public enum V2Beacon
{
    Center,
    Offset,
    Side,
    Behind
}

public enum TactileBeacon
{
    Center,
    Offset,
    Behind
}

public enum FlareBeacon
{
    Center,
    Offset,
    Side,
    Behind
}

public enum ShimmerBeacon
{
    Center,
    Offset,
    Side,
    Behind
}

public enum PingBeacon
{
    Center,
    Offset,
    Side,
    Behind
}

public enum DropBeacon
{
    Center,
    Offset,
    Behind
}

public enum SignalBeacon
{
    Center,
    Offset,
    Behind
}

public enum SignalSlowBeacon
{
    Center,
    Offset,
    Behind
}

public enum SignalVerySlowBeacon
{
    Center,
    Offset,
    Behind
}

public enum MalletBeacon
{
    Center,
    Offset,
    Behind
}

public enum MalletSlowBeacon
{
    Center,
    Offset,
    Behind
}

public enum MalletVerySlowBeacon
{
    Center,
    Offset,
    Behind
}

public enum ProximityBeacon
{
    Far,
    Near
}

public enum BeaconAccents
{
    Start,
    End
}

public enum PreviewWandAsset
{
    NoTarget
}

public static class DynamicAudioEngineAssetExtensions
{
    public static string GetDescription(this Enum asset)
    {
        return asset.ToString();
    }

    public static DynamicAudioEngineAssetSelector GetSelector(this Enum asset, AssetInput input)
    {
        // Implement the selector logic for each specific enum
        switch (asset)
        {
            case ProximityBeacon.Far:
            case ProximityBeacon.Near:
                return ProximityBeaconSelector(asset, input);
            default:
                return DynamicAudioEngineAssetSelector.Far;
        }
    }

    private static DynamicAudioEngineAssetSelector ProximityBeaconSelector(Enum asset, AssetInput input)
    {
        if (input.User == null)
        {
            return DynamicAudioEngineAssetSelector.Far;
        }

        double distance = input.User.Distance(input.Beacon);

        if (distance < 20.0)
        {
            return DynamicAudioEngineAssetSelector.Near;
        }
        else if (distance < 30.0)
        {
            return DynamicAudioEngineAssetSelector.Far;
        }
        else
        {
            return DynamicAudioEngineAssetSelector.Far;
        }
    }
}

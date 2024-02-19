using System;
using System.Collections.Generic;
using System.Linq;

public enum BeaconOption
{
    Original,
    Current,
    Flare,
    Shimmer,
    Tacticle,
    Ping,
    Drop,
    Signal,
    SignalSlow,
    SignalVerySlow,
    Mallet,
    MalletSlow,
    MalletVerySlow,
    Wand,
    Pulse
}

public static class BeaconOptionExtensions
{
    public static string GetDescription(this BeaconOption beaconOption)
    {
        switch (beaconOption)
        {
            case BeaconOption.Original: return ClassicBeacon.Description;
            case BeaconOption.Current: return V2Beacon.Description;
            case BeaconOption.Flare: return FlareBeacon.Description;
            case BeaconOption.Shimmer: return ShimmerBeacon.Description;
            case BeaconOption.Tacticle: return TactileBeacon.Description;
            case BeaconOption.Ping: return PingBeacon.Description;
            case BeaconOption.Drop: return DropBeacon.Description;
            case BeaconOption.Signal: return SignalBeacon.Description;
            case BeaconOption.SignalSlow: return SignalSlowBeacon.Description;
            case BeaconOption.SignalVerySlow: return SignalVerySlowBeacon.Description;
            case BeaconOption.Mallet: return MalletBeacon.Description;
            case BeaconOption.MalletSlow: return MalletSlowBeacon.Description;
            case BeaconOption.MalletVerySlow: return MalletVerySlowBeacon.Description;
            case BeaconOption.Wand: return HapticWandBeacon.Description;
            case BeaconOption.Pulse: return HapticPulseBeacon.Description;
            default: return string.Empty;
        }
    }

    // MARK: Initialization

    public static BeaconOption? CreateFromId(string id)
    {
        return BeaconOptionExtensions.AllCases.FirstOrDefault(beacon => beacon.GetDescription() == id);
    }

    public static IEnumerable<BeaconOption> AllCases
    {
        get
        {
            return Enum.GetValues(typeof(BeaconOption)).Cast<BeaconOption>();
        }
    }
}

public static class ClassicBeacon
{
    public const string Description = "ClassicBeacon";
}

public static class V2Beacon
{
    public const string Description = "V2Beacon";
}

public static class FlareBeacon
{
    public const string Description = "FlareBeacon";
}

// Add other beacon description classes as needed

public static class HapticWandBeacon
{
    public const string Description = "HapticWandBeacon";
}

public static class HapticPulseBeacon
{
    public const string Description = "HapticPulseBeacon";
}

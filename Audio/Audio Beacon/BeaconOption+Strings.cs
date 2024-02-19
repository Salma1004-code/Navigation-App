using System;

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
    public static string LocalizedName(this BeaconOption beaconOption)
    {
        switch (beaconOption)
        {
            case BeaconOption.Original:
                return GDLocalizedString("beacon.styles.original");
            case BeaconOption.Current:
                return GDLocalizedString("beacon.styles.current");
            case BeaconOption.Flare:
                return GDLocalizedString("beacon.styles.flare");
            case BeaconOption.Shimmer:
                return GDLocalizedString("beacon.styles.shimmer");
            case BeaconOption.Tacticle:
                return GDLocalizedString("beacon.styles.tactile");
            case BeaconOption.Ping:
                return GDLocalizedString("beacon.styles.ping");
            case BeaconOption.Drop:
                return GDLocalizedString("beacon.styles.drop");
            case BeaconOption.Signal:
                return GDLocalizedString("beacon.styles.signal");
            case BeaconOption.SignalSlow:
                return GDLocalizedString("beacon.styles.signal.slow");
            case BeaconOption.SignalVerySlow:
                return GDLocalizedString("beacon.styles.signal.very_slow");
            case BeaconOption.Mallet:
                return GDLocalizedString("beacon.styles.mallet");
            case BeaconOption.MalletSlow:
                return GDLocalizedString("beacon.styles.mallet.slow");
            case BeaconOption.MalletVerySlow:
                return GDLocalizedString("beacon.styles.mallet.very_slow");
            case BeaconOption.Wand:
                return GDLocalizedString("beacon.styles.haptic.wand");
            case BeaconOption.Pulse:
                return GDLocalizedString("beacon.styles.haptic.pulse");
            default:
                throw new ArgumentOutOfRangeException(nameof(beaconOption), beaconOption, null);
        }
    }

    private static string GDLocalizedString(string key)
    {
        // You need to implement the logic for GDLocalizedString in C# based on your requirements.
        // This is a placeholder for the localization method.
        // Example: return LocalizationManager.GetLocalizedString(key);
        throw new NotImplementedException();
    }
}

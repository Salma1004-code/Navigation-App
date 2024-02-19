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
            case BeaconOption.Original: return GDLocalizedString("beacon.styles.original");
            case BeaconOption.Current: return GDLocalizedString("beacon.styles.current");
            case BeaconOption.Flare: return GDLocalizedString("beacon.styles.flare");
            case BeaconOption.Shimmer: return GDLocalizedString("beacon.styles.shimmer");
            case BeaconOption.Tacticle: return GDLocalizedString("beacon.styles.tactile");
            case BeaconOption.Ping: return GDLocalizedString("beacon.styles.ping");
            case BeaconOption.Drop: return GDLocalizedString("beacon.styles.drop");
            case BeaconOption.Signal: return GDLocalizedString("beacon.styles.signal");
            case BeaconOption.SignalSlow: return GDLocalizedString("beacon.styles.signal.slow");
            case BeaconOption.SignalVerySlow: return GDLocalizedString("beacon.styles.signal.very_slow");
            case BeaconOption.Mallet: return GDLocalizedString("beacon.styles.mallet");
            case BeaconOption.MalletSlow: return GDLocalizedString("beacon.styles.mallet.slow");
            case BeaconOption.MalletVerySlow: return GDLocalizedString("beacon.styles.mallet.very_slow");
            case BeaconOption.Wand: return GDLocalizedString("beacon.styles.haptic.wand");
            case BeaconOption.Pulse: return GDLocalizedString("beacon.styles.haptic.pulse");
            default:
                throw new ArgumentOutOfRangeException(nameof(beaconOption), beaconOption, null);
        }
    }

    private static string GDLocalizedString(string key)
    {
        // Placeholder for your actual localization logic.
        // Replace this with the appropriate localization logic in your C# project.
        // Example: return LocalizationManager.GetLocalizedString(key);
        throw new NotImplementedException();
    }
}

public class LocalizationManager
{
    // Placeholder for actual localization logic.
    public static string GetLocalizedString(string key)
    {
        // Replace this with your real localization logic.
        // Example: fetch the localized string from a resource file, database, or another source.
        // For the sake of this example, we'll return a simple placeholder with the key itself.
        return $"Localized: {key}";
    }
}

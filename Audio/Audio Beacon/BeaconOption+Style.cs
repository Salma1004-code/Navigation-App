using System;
using System.Collections.Generic;
using System.Linq;

public enum BeaconOption
{
    Wand,
    Pulse,
    // Add other BeaconOption cases as needed
}

public static class BeaconOptionExtensions
{
    public enum Style
    {
        Standard,
        Haptic
    }

    public static Style GetStyle(this BeaconOption beaconOption)
    {
        switch (beaconOption)
        {
            case BeaconOption.Wand:
            case BeaconOption.Pulse:
                return Style.Haptic;
            default:
                return Style.Standard;
        }
    }

    public static IEnumerable<BeaconOption> AllCasesForStyle(this IEnumerable<BeaconOption> allCases, Style style)
    {
        return allCases.Where(option => option.GetStyle() == style);
    }

    // Availability

    public static bool IsAvailable(this Style style)
    {
        switch (style)
        {
            case Style.Standard:
                return true;
            case Style.Haptic:
                // Replace HapticEngine.supportsHaptics with your actual check for haptic support in C#
                return HapticEngine.SupportsHaptics;
            default:
                return false;
        }
    }

    public static IEnumerable<BeaconOption> AllAvailableCases(this IEnumerable<BeaconOption> allCases)
    {
        return allCases.Where(option => option.GetStyle().IsAvailable());
    }

    public static IEnumerable<BeaconOption> AllAvailableCasesForStyle(this IEnumerable<BeaconOption> allCases, Style style)
    {
        return allCases.AllAvailableCases().AllCasesForStyle(style);
    }
}

public static class HapticEngine
{
    // Replace this with your actual check for haptic support in C#
    public static bool SupportsHaptics => true; // Placeholder value
}

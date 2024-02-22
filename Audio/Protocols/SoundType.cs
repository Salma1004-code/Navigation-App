using System;

public enum SoundDistanceRenderingStyle
{
    Ring,
    Real
}

public static class SoundDistanceRenderingStyleExtensions
{
    public static string FormattedLog(this SoundDistanceRenderingStyle style)
    {
        switch (style)
        {
            case SoundDistanceRenderingStyle.Ring:
                // Assuming DebugSettingsContext is a class with a shared property and envRenderingDistance is a static property
                return $"ring {DebugSettingsContext.Shared.EnvRenderingDistance:F1}m";
            default:
                return style.ToString().ToLower();
        }
    }
}

public enum SoundType
{
    Standard,
    Localized,
    Relative,
    Compass
}

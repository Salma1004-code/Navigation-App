using System;

public interface ISoundBase
{
    SoundType Type { get; }
    int LayerCount { get; }
    EQParameters? EqualizerParams(int layerIndex);
}

public static class SoundBaseExtensions
{
    public static string FormattedLog(this ISoundBase soundBase)
    {
        switch (soundBase.Type)
        {
            case SoundType.Standard:
                return $"{soundBase} (Standard 2D)";
            case SoundType.Localized:
                var localizedSound = soundBase as ILocalizedSound;
                return $"{soundBase} (Style: {localizedSound.Style.FormattedLog}; Localized at {localizedSound.Location.Coordinate.Latitude:F6}, {localizedSound.Location.Coordinate.Longitude:F6})";
            case SoundType.Compass:
                var compassSound = soundBase as ICompassSound;
                return $"{soundBase} (Style: {compassSound.Style.FormattedLog}; Compass direction {compassSound.Direction.RoundToDecimalPlaces(0)}°)";
            case SoundType.Relative:
                var relativeSound = soundBase as IRelativeSound;
                return $"{soundBase} (Style: {relativeSound.Style.FormattedLog}; Relative at {relativeSound.Direction.RoundToDecimalPlaces(0)}°)";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

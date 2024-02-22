using System;
using System.Collections.Generic;

public enum AssetSelectorInputType
{
    Heading,
    Location
}

public enum DynamicAudioEngineAssetType
{
    // Define your dynamic audio asset types here
}

public interface DynamicAudioEngineAsset
{
    public delegate (DynamicAudioEngineAssetType asset, float volume)? AssetSelector(AssetSelectorInputType inputType, dynamic input);

    AssetSelector Selector { get; }

    int BeatsInPhrase { get; }
}

public static class DynamicAudioEngineAssetExtensions
{
    public static string Description<T>(this T asset) where T : DynamicAudioEngineAsset
    {
        return asset.ToString();
    }

    public static AssetSelector DefaultSelector<T>(this T asset) where T : DynamicAudioEngineAsset
    {
        switch (Enum.GetValues(typeof(DynamicAudioEngineAssetType)).Length)
        {
            // Implement logic based on the number of asset types
            case 2:
                return StandardTwoRegionSelector<T>();
            case 3:
                return StandardThreeRegionSelector<T>();
            case 4:
                return StandardFourRegionSelector<T>();
            default:
                return null;
        }
    }

    private static AssetSelector StandardTwoRegionSelector<T>() where T : DynamicAudioEngineAsset
    {
        if (Enum.GetValues(typeof(DynamicAudioEngineAssetType)).Length != 2)
            return null;

        return (inputType, input) =>
        {
            if (inputType == AssetSelectorInputType.Heading)
            {
                var userHeading = (float?)input.userHeading;
                var poiBearing = (float)input.poiBearing;

                if (!userHeading.HasValue)
                    return (DynamicAudioEngineAssetType.Second, 1.0f);

                var angle = userHeading.Value - poiBearing;

                // 45 degree window for the V1SensoryBeatOn sound
                if (angle >= 337.5 || angle <= 22.5)
                    return (DynamicAudioEngineAssetType.First, 1.0f);
                else
                    return (DynamicAudioEngineAssetType.Second, 1.0f);
            }

            return null;
        };
    }

    private static AssetSelector StandardThreeRegionSelector<T>() where T : DynamicAudioEngineAsset
    {
        if (Enum.GetValues(typeof(DynamicAudioEngineAssetType)).Length != 3)
            return null;

        return (inputType, input) =>
        {
            if (inputType == AssetSelectorInputType.Heading)
            {
                var userHeading = (float?)input.userHeading;
                var poiBearing = (float)input.poiBearing;

                if (!userHeading.HasValue)
                    return (DynamicAudioEngineAssetType.Third, 1.0f);

                var angle = userHeading.Value - poiBearing;

                if (angle >= 345 || angle <= 15)
                    return (DynamicAudioEngineAssetType.First, 1.0f);
                else if ((angle >= 235 && angle <= 345) || (angle >= 15 && angle <= 125))
                    return (DynamicAudioEngineAssetType.Second, 1.0f);
                else
                    return (DynamicAudioEngineAssetType.Third, 1.0f);
            }

            return null;
        };
    }

    private static AssetSelector StandardFourRegionSelector<T>() where T : DynamicAudioEngineAsset
    {
        if (Enum.GetValues(typeof(DynamicAudioEngineAssetType)).Length != 4)
            return null;

        return (inputType, input) =>
        {
            if (inputType == AssetSelectorInputType.Heading)
            {
                var userHeading = (float?)input.userHeading;
                var poiBearing = (float)input.poiBearing;

                if (!userHeading.HasValue)
                    return (DynamicAudioEngineAssetType.Fourth, 1.0f);

                var angle = userHeading.Value - poiBearing;

                if (angle >= 345 || angle <= 15)
                    return (DynamicAudioEngineAssetType.First, 1.0f);
                else if ((angle >= 305 && angle <= 345) || (angle >= 15 && angle <= 55))
                    return (DynamicAudioEngineAssetType.Second, 1.0f);
                else if ((angle >= 235 && angle <= 305) || (angle >= 55 && angle <= 125))
                    return (DynamicAudioEngineAssetType.Third, 1.0f);
                else
                    return (DynamicAudioEngineAssetType.Fourth, 1.0f);
            }

            return null;
        };
    }
}

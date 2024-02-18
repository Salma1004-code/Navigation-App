public enum FeatureFlag
{
    DeveloperTools,
    ExperimentConfiguration
}

public static class FeatureFlagExtensions
{
    public static bool IsEnabled(this FeatureFlag feature)
    {
        // In C#, we typically use configuration files, environment variables, or command-line arguments
        // to manage feature flags instead of conditional compilation blocks.
        // Here's a simple example using a hypothetical `Config` class:

        if (Config.AllFlagsDisabled)
        {
            // All feature flags are disabled
            return false;
        }
        else if (Config.AllFlagsEnabled)
        {
            // All feature flags are enabled
            return true;
        }
        else
        {
            // Check the given feature flag
            switch (feature)
            {
                case FeatureFlag.DeveloperTools:
                    return Config.DeveloperToolsEnabled;
                case FeatureFlag.ExperimentConfiguration:
                    return Config.ExperimentConfigurationEnabled;
                default:
                    return false;
            }
        }
    }
}

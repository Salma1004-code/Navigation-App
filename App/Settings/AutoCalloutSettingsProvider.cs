public interface IAutoCalloutSettingsProvider
{
    bool AutomaticCalloutsEnabled { get; set; }
    bool PlaceSenseEnabled { get; set; }
    bool LandmarkSenseEnabled { get; set; }
    bool MobilitySenseEnabled { get; set; }
    bool InformationSenseEnabled { get; set; }
    bool SafetySenseEnabled { get; set; }
    bool IntersectionSenseEnabled { get; set; }
    bool DestinationSenseEnabled { get; set; }
}

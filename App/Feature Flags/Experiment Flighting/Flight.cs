using System;
using System.Collections.Generic;
using System.Linq;

public class Flight
{
    public string Etag { get; set; }
    public List<ExperimentConfiguration> Configurations { get; set; }
    public Dictionary<Guid, bool> ConfigStates { get; private set; }
    public List<ExperimentDescription> ExperimentDescriptions { get; set; }

    public Flight(string etag, List<ExperimentConfiguration> configurations, Dictionary<Guid, bool> configStates)
    {
        Etag = etag;
        Configurations = configurations;
        ConfigStates = configStates;
    }

    public Flight(string etag, List<ExperimentConfiguration> configurations)
    {
        var states = configurations.ToDictionary(config => config.Uuid, config => new Random().NextDouble() < config.Probability);
        Etag = etag;
        Configurations = configurations;
        ConfigStates = states;
    }

    public bool IsActive(Guid experimentID, string locale = LocalizationContext.CurrentAppLocale)
    {
        var control = Configurations.FirstOrDefault(config => config.ExperimentIDs.Contains(experimentID));
        if (control == null || !control.Locales.Contains(locale))
        {
            return false;
        }

        return ConfigStates[control.Uuid];
    }

    public void SetIsActive(Guid configId, bool isActive)
    {
        if (!FeatureFlag.IsEnabled(FeatureFlag.ExperimentConfiguration) || !ConfigStates.ContainsKey(configId))
        {
            return;
        }

        ConfigStates[configId] = isActive;
    }
}

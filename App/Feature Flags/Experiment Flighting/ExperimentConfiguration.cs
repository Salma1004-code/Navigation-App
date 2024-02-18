using System;
using System.Collections.Generic;

public struct ExperimentConfiguration
{
    // Unique identifier for this experiment configuration
    public Guid Uuid { get; set; }

    // List of experiment IDs
    public List<Guid> ExperimentIds { get; set; }

    // Value in the range [0.0, 1.0] indicating the probability that this list
    // of experiments is enabled for any given user
    public float Probability { get; set; }

    // Locales this configuration is available in
    public List<string> Locales { get; set; }
}

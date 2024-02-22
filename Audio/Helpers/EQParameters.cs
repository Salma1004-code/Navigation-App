public class FilterBandParameters
{
    // You need to define the properties for FilterBandParameters based on your requirements
}

public class EQParameters
{
    // Array of filter parameters for each band in the filter
    public FilterBandParameters[] BandParameters { get; set; }

    // The overall gain adjustment applied to the signal, in decibels
    public float GlobalGain { get; set; }

    // Constructor
    public EQParameters(float globalGain = 0.0f, FilterBandParameters[] parameters = null)
    {
        GlobalGain = Math.Max(-96.0f, Math.Min(24.0f, globalGain));
        BandParameters = parameters ?? new FilterBandParameters[0];
    }
}

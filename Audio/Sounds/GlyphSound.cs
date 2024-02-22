using System;

public class GlyphSound : GenericSound
{
    public override string Description
    {
        get
        {
            switch (Source)
            {
                case GenericAudioSource.File:
                    return $"[{File.LastPathComponent}]";
                case GenericAudioSource.Bundle:
                    return $"[{BundleAsset.Name}]";
                default:
                    return string.Empty;
            }
        }
    }

    // Constructors, methods, and other members from GenericSound can be inherited as-is.
}

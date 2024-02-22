using System;
using AVFoundation;

public enum GenericAudioSource
{
    File,
    Bundle
}

public class GenericSound : ISynchronouslyGeneratedSound
{
    public SoundType Type { get; private set; }
    public GenericAudioSource Source { get; private set; }
    public int LayerCount { get; } = 1;
    private AVAudioPCMBuffer buffer;

    public string Description
    {
        get
        {
            switch (Source)
            {
                case GenericAudioSource.File:
                    return $"{{{File.LastPathComponent}}}";
                case GenericAudioSource.Bundle:
                    return $"{{{BundleAsset.Name}}}";
                default:
                    return string.Empty;
            }
        }
    }

    public double? Duration
    {
        get
        {
            if (buffer == null)
                return null;

            double frames = buffer.FrameLength;
            double sampleRate = buffer.Format.SampleRate;

            return frames / sampleRate;
        }
    }

    public GenericSound(StaticAudioEngineAsset bundleAsset)
    {
        Source = GenericAudioSource.Bundle;
        Type = SoundType.Standard;
        buffer = bundleAsset.Load();
    }

    public GenericSound(StaticAudioEngineAsset bundleAsset, CLLocation at)
    {
        Source = GenericAudioSource.Bundle;
        Type = SoundType.Localized;
        buffer = bundleAsset.Load();
    }

    public GenericSound(StaticAudioEngineAsset bundleAsset, double direction)
    {
        Source = GenericAudioSource.Bundle;
        Type = SoundType.Relative;
        buffer = bundleAsset.Load();
    }

    public GenericSound(StaticAudioEngineAsset bundleAsset, double compass)
    {
        Source = GenericAudioSource.Bundle;
        Type = SoundType.Compass;
        buffer = bundleAsset.Load();
    }

    public GenericSound(Uri file)
    {
        Source = GenericAudioSource.File;
        Type = SoundType.Standard;
        buffer = LoadBufferFromFile(file);
    }

    public GenericSound(Uri file, CLLocation at)
    {
        Source = GenericAudioSource.File;
        Type = SoundType.Localized;
        buffer = LoadBufferFromFile(file);
    }

    public GenericSound(Uri file, double direction)
    {
        Source = GenericAudioSource.File;
        Type = SoundType.Relative;
        buffer = LoadBufferFromFile(file);
    }

    public GenericSound(Uri file, double compass)
    {
        Source = GenericAudioSource.File;
        Type = SoundType.Compass;
        buffer = LoadBufferFromFile(file);
    }

    public AVAudioPCMBuffer GenerateBuffer(int layerIndex)
    {
        if (layerIndex != 0)
            return null;

        AVAudioPCMBuffer preloaded = buffer;
        buffer = null;
        return preloaded;
    }

    public EQParameters EqualizerParams(int layerIndex)
    {
        double gain = SettingsContext.Shared.AfxGain;

        if (gain == 0)
            return null;

        return new EQParameters(gain, new float[] { });
    }

    private AVAudioPCMBuffer LoadBufferFromFile(Uri file)
    {
        try
        {
            AVAudioFile audioFile = new AVAudioFile(file, AVAudioFile.OpenMode.Read);
            AVAudioPCMBuffer pcmBuffer = new AVAudioPCMBuffer(audioFile.ProcessingFormat, (uint)audioFile.Length);
            audioFile.ReadIntoBuffer(pcmBuffer);
            return pcmBuffer;
        }
        catch
        {
            return null;
        }
    }
}

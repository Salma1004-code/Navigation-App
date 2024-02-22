using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ConcatenatedSound : ISound
{
    public SoundType Type { get; private set; }
    public string Description { get; private set; }
    public List<ISound> ConcatenatedSounds { get; private set; }
    public int LayerCount { get; } = 1;

    private Promise<AVAudioPCMBuffer?> currentBufferPromise;
    private int currentSoundIndex = 0;

    public ConcatenatedSound(params ISound[] sounds)
    {
        if (sounds.Length == 0)
            return;

        ConcatenatedSounds = sounds.ToList();

        // All sub-sounds must have only a single channel
        if (!ConcatenatedSounds.All(channel => channel.LayerCount == 1))
            return;

        // All sub-sounds must have analogous sound types
        var type = ConcatenatedSounds[0].Type;
        var typesMatch = ConcatenatedSounds.All(channel =>
        {
            switch (channel.Type, type)
            {
                case (SoundType.Standard, SoundType.Standard):
                case (SoundType.Localized, SoundType.Localized):
                case (SoundType.Relative, SoundType.Relative):
                case (SoundType.Compass, SoundType.Compass):
                    return true;
                default:
                    return false;
            }
        });

        if (!typesMatch)
            return;

        Type = type;

        if (ConcatenatedSounds.First() is ISound first)
        {
            Description = "[" + ConcatenatedSounds.Skip(1).Aggregate(first.Description, (current, sound) => current + ", " + sound.Description) + "]";
        }
        else
        {
            Description = "[]";
        }
    }

    public Promise<AVAudioPCMBuffer?> NextBuffer(int layerIndex)
    {
        if (layerIndex != 0)
        {
            return new Promise<AVAudioPCMBuffer?>(resolver => resolver(null));
        }

        return new Promise<AVAudioPCMBuffer?>(resolver =>
        {
            if (this == null)
            {
                resolver(null);
                return;
            }

            NextBufferForCurrentIndex(resolver);
        });
    }

    private void NextBufferForCurrentIndex(Promise<AVAudioPCMBuffer?>.Resolver resolver)
    {
        currentBufferPromise = ConcatenatedSounds[currentSoundIndex].NextBuffer(0) as Promise<AVAudioPCMBuffer?>;
        currentBufferPromise?.Then(buffer =>
        {
            if (buffer != null)
            {
                resolver(buffer);
                return;
            }

            if (this != null && currentSoundIndex < ConcatenatedSounds.Count - 1)
            {
                currentSoundIndex += 1;
                currentBufferPromise = (ConcatenatedSounds[currentSoundIndex].NextBuffer(0) as Promise<AVAudioPCMBuffer?>);
                currentBufferPromise?.Then(nextBuffer => resolver(nextBuffer));
            }
            else
            {
                resolver(null);
            }
        });
    }

    public EQParameters EqualizerParams(int layerIndex)
    {
        if (layerIndex != 0)
        {
            return null;
        }

        return ConcatenatedSounds.Select(channel => channel.EqualizerParams(0)).FirstOrDefault(params => params != null);
    }
}

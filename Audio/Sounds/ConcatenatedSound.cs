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
                case (SoundType.Localized, SoundType.Localized

using AVFoundation;
using CoreLocation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class TTSSound : ISound
{
    public SoundType Type { get; }
    public string Text { get; }
    public int LayerCount { get; set; } = 1;
    public string Description => $"\"{Text}\"";

    private List<AVAudioPCMBuffer> Buffers { get; } = new List<AVAudioPCMBuffer>();
    private List<Func<AVAudioPCMBuffer, Task>> Resolvers { get; } = new List<Func<AVAudioPCMBuffer, Task>>();

    private IDisposable Cancellable { get; set; }
    private TaskCompletionSource<object> Completion { get; set; }

    private readonly object lockObject = new object();
    private readonly Queue<AVAudioPCMBuffer> queue = new Queue<AVAudioPCMBuffer>();

    private struct VoiceEQ
    {
        public string Id { get; set; }
        public EQParameters Filter { get; set; }
    }

    public static Dictionary<string, EQParameters> Filters { get; } = LoadFilters();

    private static Dictionary<string, EQParameters> LoadFilters()
    {
        string path = "voiceFilters.json"; // Adjust the path accordingly
        try
        {
            string json = File.ReadAllText(path);
            var filterList = JsonConvert.DeserializeObject<List<VoiceEQ>>(json);
            return filterList.ToDictionary(item => item.Id, item => item.Filter);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to parse voiceFilters.json file! {ex.Message}");
            return new Dictionary<string, EQParameters>();
        }
    }

    public TTSSound(string text, SoundType type)
    {
        Text = text;
        Type = type;
    }

    public TTSSound(string text) : this(text, SoundType.Standard) { }

    public TTSSound(string text, CLLocation at) : this(text, SoundType.Localized, at) { }

    public TTSSound(string text, CLLocationDirection direction) : this(text, SoundType.Relative, direction) { }

    public TTSSound(string text, CLLocationDirection compass) : this(text, SoundType.Compass, compass) { }

    public void StopRendering()
    {
        Cancellable?.Dispose();
    }

    private void ResolveBuffer(AVAudioPCMBuffer buffer)
    {
        lock (lockObject)
        {
            if (Resolvers.Count > 0)
            {
                var resolver = Resolvers.First();
                Resolvers.Remove(resolver);
                resolver(buffer);
            }
            else
            {
                queue.Enqueue(buffer);
            }
        }
    }

    public Task<AVAudioPCMBuffer?> NextBuffer(int index)
    {
        if (index != 0)
        {
            return Task.FromResult<AVAudioPCMBuffer?>(null);
        }

        if (Cancellable == null && Completion == null)
        {
            try
            {
                var ttsAudioBufferPublisher = new TTSAudioBufferPublisher(Text);
                if (ttsAudioBufferPublisher == null)
                {
                    return Task.FromResult<AVAudioPCMBuffer?>(null);
                }

                Cancellable = ttsAudioBufferPublisher
                    .ObserveOn(TaskPoolScheduler.Default)
                    .Subscribe(
                        buffer => ResolveBuffer(buffer),
                        error => Console.WriteLine($"Error: {error}"),
                        () => ResolveBuffer(null)
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Task.FromResult<AVAudioPCMBuffer?>(null);
            }
        }

        AVAudioPCMBuffer nextBuffer;
        lock (lockObject)
        {
            if (queue.Count > 0)
            {
                nextBuffer = queue.Dequeue();
            }
            else if (Completion != null)
            {
                return Task.FromResult<AVAudioPCMBuffer?>(null);
            }
            else
            {
                var tcs = new TaskCompletionSource<AVAudioPCMBuffer?>();
                Resolvers.Add(async buffer => tcs.SetResult(buffer));
                return tcs.Task;
            }
        }

        return Task.FromResult<AVAudioPCMBuffer?>(nextBuffer);
    }

    public EQParameters EqualizerParams(int layerIndex)
    {
        if (layerIndex != 0)
        {
            return null;
        }

        double gain = SettingsContext.Shared.TtsGain;

        string id = SettingsContext.Shared.VoiceId;
        if (!string.IsNullOrEmpty(id))
        {
            var parameters = Filters.ContainsKey(id) ? Filters[id].BandParameters : null;

            if (parameters == null && gain == 0)
            {
                return null;
            }

            return new EQParameters(gain, parameters ?? Enumerable.Empty<Parameter>());
        }

        var defaultVoiceId = TTSConfigHelper.DefaultVoiceForLocale(LocalizationContext.CurrentAppLocale)?.Identifier;
        var defaultParameters = Filters.ContainsKey(defaultVoiceId) ? Filters[defaultVoiceId].BandParameters : null;

        if (defaultParameters == null && gain == 0)
        {
            return null;
        }

        return new EQParameters(gain, defaultParameters ?? Enumerable.Empty<Parameter>());
    }
}

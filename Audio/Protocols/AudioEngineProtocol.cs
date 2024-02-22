using AVFoundation;
using CoreLocation;
using System;

public interface IAudioEngineDelegate
{
    void DidFinishPlaying();
}

public interface IAudioEngineProtocol
{
    AVAudioSession Session { get; }
    string OutputType { get; }
    IAudioEngineDelegate Delegate { get; set; }
    bool IsRecording { get; }
    bool IsDiscreteAudioPlaying { get; }
    bool IsInMonoMode { get; }
    bool MixWithOthers { get; set; }
    static Uri RecordingDirectory { get; }

    void Start(bool isRestarting = false, bool activateAudioSession = true);
    void Stop();
    AudioPlayerIdentifier? Play<T>(T sound, Heading? heading) where T : IDynamicSound;
    AudioPlayerIdentifier? Play(ISynchronouslyGeneratedSound sound);
    AudioPlayerIdentifier? PlayLooped(ISynchronouslyGeneratedSound looped);
    void Play(ISound sound, Action<bool> callback);
    void Play(ISounds sounds, Action<bool> callback);
    void Finish(AudioPlayerIdentifier dynamicPlayerId);
    void Stop(AudioPlayerIdentifier id);
    void StopDiscrete(ISound with);
    void UpdateUserLocation(CLLocation location);
    void StartRecording();
    void StopRecording();
    void EnableSpeakerMode(Action<AVAudioSessionPortOverride> handler);
    void DisableSpeakerMode(Action<AVAudioSessionPortOverride> handler);
}

public static class AudioEngineProtocolExtensions
{
    public static void Start(this IAudioEngineProtocol audioEngine, bool isRestarting = false, bool activateAudioSession = true)
    {
        audioEngine.Start(isRestarting, activateAudioSession);
    }

    public static AudioPlayerIdentifier? Play<T>(this IAudioEngineProtocol audioEngine, T sound) where T : IDynamicSound
    {
        return audioEngine.Play(sound, heading: null);
    }

    public static void Play(this IAudioEngineProtocol audioEngine, ISound sound)
    {
        audioEngine.Play(sound, callback: null);
    }

    public static void Play(this IAudioEngineProtocol audioEngine, ISounds sounds)
    {
        audioEngine.Play(sounds, callback: null);
    }

    public static void StopDiscrete(this IAudioEngineProtocol audioEngine, ISound with = null)
    {
        audioEngine.StopDiscrete(with);
    }
}

using System;
using System.Collections.Generic;
using AVFoundation;
using CoreLocation;

public interface IAudioPlayer
{
    // A unique identifier for the AudioPlayer
    Guid Id { get; }

    // Array of nodes and their associated formats
    List<PreparableAudioLayer> Layers { get; }

    SoundBase Sound { get; }
    AudioPlayerState State { get; }
    bool IsPlaying { get; }
    bool Is3D { get; }
    float Volume { get; }

    void Prepare(AVAudioEngine engine, Action<bool> completion);
    void UpdateConnectionState(AudioPlayerConnectionState state);
    void Play(Heading? userHeading, CLLocation? userLocation);
    bool ResumeIfNecessary();
    void Stop();
}

public enum AudioPlayerState
{
    NotPrepared,
    Preparing,
    Prepared
}

public enum AudioPlayerConnectionState
{
    NotConnected,
    Unknown,
    Connected
}

public class AudioPlayerExtension : IAudioPlayer
{
    public bool Is3D
    {
        get
        {
            return Sound.Type == SoundType.Standard ? false : true;
        }
    }

    public void Play(Heading? userHeading = null, CLLocation? userLocation = null)
    {
        Play(userHeading, userLocation);
    }
}

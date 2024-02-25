public interface IEnvironmentSettingsProvider
{
    AVAudio3DMixingRenderingAlgorithm EnvRenderingAlgorithm { get; set; }
    double EnvRenderingDistance { get; set; }
    
    bool EnvRenderingReverbEnable { get; set; }
    AVAudioUnitReverbPreset EnvRenderingReverbPreset { get; set; }
    float EnvRenderingReverbBlend { get; set; }
    float EnvRenderingReverbLevel { get; set; }
    
    bool EnvReverbFilterActive { get; set; }
    float EnvReverbFilterBandwidth { get; set; }
    bool EnvReverbFilterBypass { get; set; }
    AVAudioUnitEQFilterType EnvReverbFilterType { get; set; }
    float EnvReverbFilterFrequency { get; set; }
    float EnvReverbFilterGain { get; set; }
}

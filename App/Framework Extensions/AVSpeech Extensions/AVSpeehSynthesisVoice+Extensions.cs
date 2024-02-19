public static class AVSpeechSynthesisVoiceExtensions
{
    public static bool HasEnhancedVersion(this AVSpeechSynthesisVoice voice)
    {
        return voice.Identifier.Contains("com.apple.ttsbundle") && voice.Identifier.Contains("compact") && !voice.Identifier.Contains("siri");
    }

    public static bool HasEnhancedVersionDownloaded(this AVSpeechSynthesisVoice voice)
    {
        if (!voice.HasEnhancedVersion())
        {
            return false;
        }

        return AVSpeechSynthesisVoice.SpeechVoices().Any(v => v.Identifier == voice.Identifier.Replace("compact", "premium"));
    }
}

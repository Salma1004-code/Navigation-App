using System.Threading.Tasks;

public interface ISound : ISoundBase
{
    /// <summary>
    /// Generates the next PCM audio buffer for this sound. This method allows for asynchronous buffer generation.
    /// </summary>
    /// <param name="layer">The layer for which the buffer is requested.</param>
    /// <returns>A task representing the asynchronous operation, returning the next PCM audio buffer for the sound.</returns>
    Task<AVAudioPCMBuffer?> NextBuffer(int layer);
}

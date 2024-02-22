using System.Threading.Tasks;

public interface ISynchronouslyGeneratedSound : ISound
{
    AVAudioPCMBuffer GenerateBuffer(int layer);
}

public static class SynchronouslyGeneratedSoundExtensions
{
    public static Task<AVAudioPCMBuffer?> NextBuffer(this ISynchronouslyGeneratedSound sound, int channel)
    {
        // Generate the buffer and immediately complete the task.
        return Task.FromResult(sound.GenerateBuffer(channel));
    }
}

using System;
using System.IO;
using AVFoundation;
using Foundation;

public interface IAudioEngineAsset
{
    string Name { get; }
    string Type { get; }
    AVAudioPCMBuffer Load();
}

public static class AudioEngineAssetExtensions
{
    public static string Name(this IAudioEngineAsset asset) => asset.Name;

    public static string Type(this IAudioEngineAsset asset) => asset.Type;

    public static string TypeOrDefault(this IAudioEngineAsset asset) => "wav";

    public static AVAudioPCMBuffer Load(this IAudioEngineAsset asset)
    {
        try
        {
            string path = NSBundle.MainBundle.PathForResource(asset.Name, asset.Type);
            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine($"Audio asset could not be found (name: {asset.Name}, type: {asset.Type})");
                return null;
            }

            var url = new NSUrl(path);
            if (url == null)
            {
                Console.WriteLine($"Audio asset URL not valid (name: {asset.Name}, type: {asset.Type})");
                return null;
            }

            var file = new AVAudioFile(url);

            if (!(AVAudioPCMBuffer.Create(file.ProcessingFormat, (uint)file.Length, out var buffer)))
            {
                return null;
            }

            file.ReadIntoBuffer(buffer);

            return buffer;
        }
        catch (Exception)
        {
            Console.WriteLine($"Audio asset could not be loaded (name: {asset.Name}, type: {asset.Type})");
            return null;
        }
    }
}

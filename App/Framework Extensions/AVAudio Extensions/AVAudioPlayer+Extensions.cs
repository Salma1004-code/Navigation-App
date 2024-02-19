using System;
using System.IO;

public class AVAudioPlayer
{
    public string FilePath { get; set; }

    public AVAudioPlayer(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", filePath);
        }

        FilePath = filePath;
    }

    public static AVAudioPlayer Player(string filename, string fileTypeHint = "wav", bool allowSharedFolder = true)
    {
        // Try to load from shared folder
        if (allowSharedFolder)
        {
            try
            {
                return new AVAudioPlayer(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{filename}.{fileTypeHint}"));
            }
            catch (FileNotFoundException)
            {
                // Ignore and try next
            }
        }

        // Try to load from asset catalog
        // This is platform-specific and may need to be adjusted based on your application's structure
        try
        {
            return new AVAudioPlayer(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", $"{filename}.{fileTypeHint}"));
        }
        catch (FileNotFoundException)
        {
            // Ignore and try next
        }

        // Try to load from main bundle
        try
        {
            return new AVAudioPlayer(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{filename}.{fileTypeHint}"));
        }
        catch (FileNotFoundException)
        {
            // Ignore and return null
        }

        return null;
    }
}

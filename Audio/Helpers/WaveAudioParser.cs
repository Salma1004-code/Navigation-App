using System;
using System.Text;

public class WaveAudioParser
{
    private const string RiffMarker = "RIFF";
    private const string FmtMarker = "fmt ";
    private const string DataMarker = "data";

    private static readonly Range FmtSampleRateByteRange = new Range(12, 12 + sizeof(uint));
    private static readonly Range ChunkMarkerByteRange = new Range(0, 4);
    private static readonly Range SubchunkSizeByteRange = new Range(4, 4 + sizeof(uint));

    private const int ChunkHeaderSize = 12;
    private const int SubchunkHeaderSize = 8;

    public static byte[] GetAudioData(byte[] data)
    {
        var dataBytes = GetChunkWithMarker(data, DataMarker);
        if (dataBytes == null || SubchunkHeaderSize >= dataBytes.Length)
        {
            return null;
        }

        return dataBytes.AsSpan(SubchunkHeaderSize).ToArray();
    }

    public static uint? GetSampleRate(byte[] data)
    {
        var fmtBytes = GetChunkWithMarker(data, FmtMarker);
        if (fmtBytes == null || FmtSampleRateByteRange.End > fmtBytes.Length)
        {
            return null;
        }

        var sampleRateBytes = fmtBytes[FmtSampleRateByteRange.Start..FmtSampleRateByteRange.End];
        return BitConverter.ToUInt32(sampleRateBytes, 0);
    }

    private static byte[] GetChunkWithMarker(byte[] data, string marker)
    {
        var (currentMarker, currentSize) = ParseChunk(data);
        if (currentMarker == marker)
        {
            return currentSize <= data.Length ? data[..currentSize] : null;
        }

        if (currentSize >= data.Length)
        {
            return null;
        }

        return GetChunkWithMarker(data[currentSize..], marker);
    }

    private static (string?, int)? ParseChunk(byte[] data)
    {
        if (ChunkMarkerByteRange.End > data.Length)
        {
            return null;
        }

        var markerBytes = data[ChunkMarkerByteRange.Start..ChunkMarkerByteRange.End];
        var marker = Encoding.UTF8.GetString(markerBytes);

        if (marker == RiffMarker)
        {
            if (ChunkHeaderSize > data.Length)
            {
                return null;
            }

            return (marker, ChunkHeaderSize);
        }

        if (SubchunkSizeByteRange.End > data.Length)
        {
            return null;
        }

        var sizeBytes = data[SubchunkSizeByteRange.Start..SubchunkSizeByteRange.End];
        var size = BitConverter.ToUInt32(sizeBytes, 0);

        if (size >= data.Length)
        {
            return null;
        }

        return (marker, SubchunkHeaderSize + (int)size);
    }
}

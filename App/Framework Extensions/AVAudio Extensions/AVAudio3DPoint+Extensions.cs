using System;

public class AVAudio3DPoint
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public AVAudio3DPoint(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public AVAudio3DPoint(double bearing, double distance = 1.0)
    {
        // Convert to radians and correct for the difference between the real world's coordinate system and the
        // virtual world's coordinate system.
        double radians = DegreesToRadians(bearing) - Math.PI / 2;

        if (radians < 0)
        {
            radians += 2 * Math.PI;
        }

        X = (float)(distance * Math.Cos(radians));
        Y = 0.0f;
        Z = (float)(distance * Math.Sin(radians));
    }

    private double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }
}

using System;

public struct DirectionRange
{
    public double Start { get; }
    public double End { get; }

    public DirectionRange(double start, double end)
    {
        if (start < 0 || start > 360 || end < 0 || end > 360)
        {
            throw new ArgumentOutOfRangeException("Direction values must be between 0 and 360 degrees.");
        }

        Start = start;
        End = end;
    }

    public DirectionRange(double direction, double windowRange)
    {
        double halfWindow = windowRange / 2;
        double adjustedStart = direction - halfWindow;
        double adjustedEnd = direction + halfWindow;

        // Normalize values to the range [0, 360)
        adjustedStart = (adjustedStart < 0) ? adjustedStart + 360 : adjustedStart;
        adjustedEnd = (adjustedEnd >= 360) ? adjustedEnd - 360 : adjustedEnd;

        Start = adjustedStart;
        End = adjustedEnd;
    }

    public bool Contains(double direction)
    {
        if (End > Start)
        {
            return direction >= Start && direction <= End;
        }
        else
        {
            return direction >= Start || direction <= End;
        }
    }
}

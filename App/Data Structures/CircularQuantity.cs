public struct CircularQuantity : IComparable<CircularQuantity>
{
    public double ValueInDegrees { get; }
    public double ValueInRadians { get; }

    public CircularQuantity(double valueInDegrees)
    {
        ValueInDegrees = valueInDegrees;
        ValueInRadians = valueInDegrees * Math.PI / 180.0;
    }

    public CircularQuantity FromRadians(double valueInRadians)
    {
        double valueInDegrees = valueInRadians * 180.0 / Math.PI;
        return new CircularQuantity(valueInDegrees);
    }

    public CircularQuantity Normalized()
    {
        double constant = 1.0;

        if (Math.Abs(ValueInDegrees) > 360.0)
        {
            constant = Math.Ceiling(Math.Abs(ValueInDegrees) / 360.0);
        }

        double nValueInDegrees = (ValueInDegrees + (constant * 360.0)) % 360.0;
        return new CircularQuantity(nValueInDegrees);
    }

    public int CompareTo(CircularQuantity other)
    {
        return ValueInDegrees.CompareTo(other.ValueInDegrees);
    }

    public static bool operator ==(CircularQuantity lhs, CircularQuantity rhs)
    {
        return lhs.Normalized().ValueInDegrees == rhs.Normalized().ValueInDegrees;
    }

    public static bool operator !=(CircularQuantity lhs, CircularQuantity rhs)
    {
        return !(lhs == rhs);
    }

    public static bool operator >(CircularQuantity lhs, CircularQuantity rhs)
    {
        return lhs.Normalized().ValueInDegrees > rhs.Normalized().ValueInDegrees;
    }

    public static bool operator <(CircularQuantity lhs, CircularQuantity rhs)
    {
        return lhs.Normalized().ValueInDegrees < rhs.Normalized().ValueInDegrees;
    }

    public static CircularQuantity operator +(CircularQuantity lhs, CircularQuantity rhs)
    {
        double sum = lhs.Normalized().ValueInDegrees + rhs.Normalized().ValueInDegrees;
        return new CircularQuantity(sum).Normalized();
    }

    public static CircularQuantity operator -(CircularQuantity lhs, CircularQuantity rhs)
    {
        double difference = lhs.Normalized().ValueInDegrees - rhs.Normalized().ValueInDegrees;
        return new CircularQuantity(difference).Normalized();
    }

    public static CircularQuantity operator -(CircularQuantity value)
    {
        double valueInDegrees = value.ValueInDegrees;
        return new CircularQuantity(-valueInDegrees).Normalized();
    }

    public override string ToString()
    {
        return ValueInDegrees.ToString();
    }
}

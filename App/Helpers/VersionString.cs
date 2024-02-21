public struct VersionString : IEquatable<VersionString>, IComparable<VersionString>
{
    public int Major { get; }
    public int Minor { get; }
    public int Revision { get; }
    public int? Build { get; }

    public VersionString(string versionString)
    {
        var parts = versionString?.Split('.') ?? new string[0];
        Major = parts.Length > 0 ? int.Parse(parts[0]) : 0;
        Minor = parts.Length > 1 ? int.Parse(parts[1]) : 0;
        Revision = parts.Length > 2 ? int.Parse(parts[2]) : 0;
        Build = parts.Length > 3 ? int.Parse(parts[3]) : (int?)null;
    }

    public override string ToString()
    {
        return Build.HasValue ? $"{Major}.{Minor}.{Revision}.{Build}" : $"{Major}.{Minor}.{Revision}";
    }

    public bool Equals(VersionString other)
    {
        return Major == other.Major && Minor == other.Minor && Revision == other.Revision && Build == other.Build;
    }

    public int CompareTo(VersionString other)
    {
        var majorComparison = Major.CompareTo(other.Major);
        if (majorComparison != 0) return majorComparison;

        var minorComparison = Minor.CompareTo(other.Minor);
        if (minorComparison != 0) return minorComparison;

        var revisionComparison = Revision.CompareTo(other.Revision);
        if (revisionComparison != 0) return revisionComparison;

        return (Build ?? 0).CompareTo(other.Build ?? 0);
    }
}

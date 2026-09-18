namespace AquaVitae.Layouts.Types;

public readonly record struct Length(float Points) : IComparable<Length>
{
    private const int PointsPerInch = 72;
    private const float MillimetersPerInch = 25.4f;
    private const float MillimetersPerPoint = MillimetersPerInch / PointsPerInch;
    
    public float Inches => Points / PointsPerInch;
    public float Millimeters => Points * MillimetersPerPoint;

    public static Length FromPoints(float points)
    {
        return new Length(points);
    }

    public static Length FromMillimeters(float millimeters)
    {
        return new Length(millimeters / MillimetersPerPoint);
    }

    public static Length FromInches(float inches)
    {
        return new Length(inches * PointsPerInch);
    }

    public int CompareTo(Length other)
    {
        return Points.CompareTo(other.Points);
    }

    public static Length operator +(Length left, Length right)
    {
        return new Length(left.Points + right.Points);
    }

    public static Length operator -(Length left, Length right)
    {
        return new Length(left.Points - right.Points);
    }

    public static Length operator -(Length value)
    {
        return new Length(-value.Points);
    }

    public static Length operator *(Length left, float right)
    {
        return new Length(left.Points * right);
    }

    public static Length operator *(float left, Length right)
    {
        return new Length(left * right.Points);
    }

    public static Length operator /(Length left, float right)
    {
        return new Length(left.Points / right);
    }
    
    public static float operator /(Length left, Length right)
    {
        return left.Points / right.Points;
    }

    public static implicit operator Length(float value)
    {
        return new Length(value);
    }
}
namespace AquaVitae.Layouts.Types;

/// <summary>
/// Represents lengths measured in typographical points.
/// This type follows the PostScript / CSS pt standard of 72 ppi.
/// </summary>
/// <param name="Points"></param>
public readonly record struct PrintPoint(float Points) : IComparable<PrintPoint>
{
    private const int PointsPerInch = 72;
    private const float MillimetersPerInch = 25.4f;
    private const float MillimetersPerPoint = MillimetersPerInch / PointsPerInch;
    
    public float Inches => Points / PointsPerInch;
    public float Millimeters => Points * MillimetersPerPoint;

    public static PrintPoint FromPoints(float points)
    {
        return new PrintPoint(points);
    }

    public static PrintPoint FromMillimeters(float millimeters)
    {
        return new PrintPoint(millimeters / MillimetersPerPoint);
    }

    public static PrintPoint FromInches(float inches)
    {
        return new PrintPoint(inches * PointsPerInch);
    }

    public int CompareTo(PrintPoint other)
    {
        return Points.CompareTo(other.Points);
    }

    public static PrintPoint operator +(PrintPoint left, PrintPoint right)
    {
        return new PrintPoint(left.Points + right.Points);
    }

    public static PrintPoint operator -(PrintPoint left, PrintPoint right)
    {
        return new PrintPoint(left.Points - right.Points);
    }

    public static PrintPoint operator -(PrintPoint value)
    {
        return new PrintPoint(-value.Points);
    }

    public static PrintPoint operator *(PrintPoint left, float right)
    {
        return new PrintPoint(left.Points * right);
    }

    public static PrintPoint operator *(float left, PrintPoint right)
    {
        return new PrintPoint(left * right.Points);
    }

    public static PrintPoint operator /(PrintPoint left, float right)
    {
        return new PrintPoint(left.Points / right);
    }
    
    public static float operator /(PrintPoint left, PrintPoint right)
    {
        return left.Points / right.Points;
    }

    public static implicit operator PrintPoint(float value)
    {
        return new PrintPoint(value);
    }
}
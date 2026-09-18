using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;

namespace AquaVitae.Layouts.Types;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 4)]
public readonly struct Color : IEquatable<Color>
{
    // Most systems are little-endian. It's therefore advantageous
    // to store the bytes in little-endian by default.
    [field: FieldOffset(0)]
    public byte Blue { get; }
    [field: FieldOffset(1)]
    public byte Green { get; }
    [field: FieldOffset(2)]
    public byte Red { get; }
    [field: FieldOffset(3)]
    public byte Alpha { get; }
    
    /// <summary>
    /// Represents the raw uint value of the color, stored as ARGB.
    /// </summary>
    [field: FieldOffset(0)]
    public uint Raw => BitConverter.IsLittleEndian ? field : BinaryPrimitives.ReverseEndianness(field);
    
    public Color(byte red, byte green, byte blue, byte alpha = 255)
    {
        Blue = blue;
        Green = green;
        Red = red;
        Alpha = alpha;
    }

    public Color(uint color)
    {
        Raw = BitConverter.IsLittleEndian ? color : BinaryPrimitives.ReverseEndianness(color);
    }
    
    public override int GetHashCode()
    {
        return unchecked((int)Raw);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Color other && Equals(other);
    }

    public bool Equals(Color other)
    {
        return Raw == other.Raw;
    }

    public static bool operator ==(Color left, Color right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Color left, Color right)
    {
        return !(left == right);
    }
    
    public override string ToString() => ToString(ColorFormats.Argb);
    
    public string ToString(ColorFormats format)
    {
        var raw = Raw;
        
        switch (format)
        {
            case ColorFormats.Rgb:
                raw &= 0x00FFFFFFu;
                return raw.ToString("X6");
            case ColorFormats.Argb:
                return raw.ToString("X8");
            case ColorFormats.Rgba:
                raw = (raw << 8) | (raw >> 24);
                return raw.ToString("X8");
            case ColorFormats.Bgra:
                return BinaryPrimitives.ReverseEndianness(raw).ToString("X8");
            case ColorFormats.Bgr:
                raw = BinaryPrimitives.ReverseEndianness(raw) >> 8;
                return raw.ToString("X6");
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }
    
    public static Color FromString(string color, ColorFormats format = ColorFormats.Rgba)
    {
        ReadOnlySpan<char> span = color.StartsWith('#') ? color[1..] : color;

        uint rawValue;
        switch (format)
        {
            case ColorFormats.Rgb when span.Length == 6:
            case ColorFormats.Rgba when span.Length == 6:
                rawValue = uint.Parse(span, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                rawValue |= 0xFF000000u;
                break;
            case ColorFormats.Rgba when span.Length == 8:
                rawValue = uint.Parse(span, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                rawValue = (rawValue >> 8) | (rawValue << 24);
                break;
            case ColorFormats.Argb when span.Length == 8:
                rawValue = uint.Parse(span, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                break;
            case ColorFormats.Bgra when span.Length == 8:
                rawValue = uint.Parse(span, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                rawValue = BinaryPrimitives.ReverseEndianness(rawValue);
                break;
            case ColorFormats.Bgr when span.Length == 6:
                rawValue = uint.Parse(span, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                rawValue = BinaryPrimitives.ReverseEndianness(rawValue) >> 8 | 0xFF000000u;
                break;
            default:
                throw new FormatException($"Unable to parse color {color} for format {format}");
        }

        return new Color(rawValue);
    }
}

public enum ColorFormats
{
    Rgb = 0,
    Rgba = 1,
    Argb = 2,
    Bgra = 3,
    Bgr = 4
}
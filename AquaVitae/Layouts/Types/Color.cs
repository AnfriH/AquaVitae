using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AquaVitae.Layouts.Types;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
public readonly struct Color(byte red, byte green, byte blue, byte alpha = 255) : IEquatable<Color>
{
    // Most systems are little-endian. It's therefore advantageous
    // to store the bytes in little-endian by default.
    public byte Blue { get; } = blue;
    public byte Green { get; } = green;
    public byte Red { get; } = red;
    public byte Alpha { get; } = alpha;
    
    /// <summary>
    /// Represents the raw uint value of the color, stored as ARGB.
    /// </summary>
    public uint Raw => BitConverter.IsLittleEndian
            ? Unsafe.BitCast<Color, uint>(this)
            : BinaryPrimitives.ReverseEndianness(Unsafe.BitCast<Color, uint>(this));

    /// <summary>
    /// Provides the hexadecimal representation of the color in ARGB format.
    /// </summary>
    public override string ToString()
    {
        return Raw.ToString("X8");
    }
    
    public string ToRgbaString() => ToString();

    public string ToRgbString()
    {
        return (Raw & 0x00FFFFFFu).ToString("X6");
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
}
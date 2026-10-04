using AquaVitae.Layouts.Types;
using DocumentFormat.OpenXml;

namespace AquaVitae.Render.Docx;

public static class DocxRendererExtensions
{
    public const int TwipsPerPoint = 20;
    public const int EmusPerPoint = 12700;
    
    extension(PrintPoint pt)
    {
        public static PrintPoint FromTwips(float twips) => new(twips / TwipsPerPoint);
        public float Twips => pt.Points * TwipsPerPoint;
        public float Emus => pt.Points * EmusPerPoint;
        public uint ToTwipsUInt() => checked((uint)MathF.Round(pt.Twips));
        public uint ToHalfPointsUint() => checked((uint)MathF.Round(pt.Points * 2));
        public short ToTwipShort() => checked((short)MathF.Round(pt.Twips));
        public int ToTwipsInt() => checked((int)MathF.Round(pt.Twips));
        public StringValue ToTwipsString() => new(pt.ToTwipsInt().ToString());
        public StringValue ToHalfPointsString() => new(pt.ToHalfPointsUint().ToString());
        public long ToEmusLong() => checked((long)MathF.Round(pt.Emus));
    }
}
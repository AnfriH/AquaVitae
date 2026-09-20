using AquaVitae.Layouts.Types;
using DocumentFormat.OpenXml;

namespace AquaVitae.Rendering.Docx;

public static class DocxRendererExtensions
{
    public const int TwipsPerPoint = 20;
    
    extension(PrintPoint pt)
    {
        public float Twips => pt.Points * TwipsPerPoint;
        public uint ToTwipsUInt() => checked((uint)MathF.Round(pt.Twips));
        public uint ToHalfPointsUint() => checked((uint)MathF.Round(pt.Points * 2));
        public short ToTwipShort() => checked((short)MathF.Round(pt.Twips));
        public int ToTwipsInt() => checked((int)MathF.Round(pt.Twips));
        public StringValue ToTwipsString() => new(pt.ToTwipsInt().ToString());
        public StringValue ToHalfPointsString() => new(pt.ToHalfPointsUint().ToString());
    }
}
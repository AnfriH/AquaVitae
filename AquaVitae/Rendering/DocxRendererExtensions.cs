using AquaVitae.Layouts.Types;
using DocumentFormat.OpenXml;

namespace AquaVitae.Rendering;

public static class DocxRendererExtensions
{
    public const int TwipsPerPoint = 20;
    
    extension(PrintPoint l)
    {
        public float Twips => l.Points * TwipsPerPoint;
        public uint ToTwipsUInt() => checked((uint)MathF.Round(l.Twips));
        public short ToTwipShort() => checked((short)MathF.Round(l.Twips));
        public int ToTwipsInt() => checked((int)MathF.Round(l.Twips));
        public StringValue ToTwipsString() => new(l.ToTwipsInt().ToString());
    }
}
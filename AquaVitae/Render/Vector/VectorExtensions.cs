using AquaVitae.Layouts.Types;
using VectSharp;

namespace AquaVitae.Render.Vector;

public static class VectorExtensions
{
    extension(Color color)
    {
        public Colour ToVectSharpColor()
        {
            return Colour.FromRgba(color.Red, color.Green, color.Blue, color.Alpha);
        }
    }
}
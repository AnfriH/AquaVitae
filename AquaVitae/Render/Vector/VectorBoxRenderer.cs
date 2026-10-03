using AquaVitae.Layouts;
using AquaVitae.Layouts.Types;
using VectSharp;

namespace AquaVitae.Render.Vector;

public sealed class VectorBoxRenderer
{
    public PrintPoint RenderBox(BoxLayout boxLayout, Graphics graphics, PrintPoint width, PrintPoint? height)
    {
        var renderHeight = height.HasValue && height.Value < boxLayout.Height
            ? height.Value
            : boxLayout.Height;
        if (boxLayout.FillColor == null) return renderHeight;
        
        var renderWidth = boxLayout.Width < width 
            ? boxLayout.Width 
            : width;

        graphics.FillRectangle(
            0,
            0,
            renderWidth.Points,
            renderHeight.Points,
            boxLayout.FillColor.Value.ToVectSharpColor()
        );
        return renderHeight;
    }
}
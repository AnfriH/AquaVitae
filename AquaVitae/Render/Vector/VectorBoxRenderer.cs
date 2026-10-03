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
            : boxLayout.Height ?? PrintPoint.Zero;
        if (boxLayout.FillColor == null || renderHeight.Points == 0) return renderHeight;
        
        var renderWidth = boxLayout.Width != null && boxLayout.Width.Value < width 
            ? boxLayout.Width.Value 
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
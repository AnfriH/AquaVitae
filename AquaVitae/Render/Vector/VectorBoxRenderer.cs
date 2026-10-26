using AquaVitae.Layouts;
using AquaVitae.Layouts.Types;
using VectSharp;

namespace AquaVitae.Render.Vector;

public sealed class VectorBoxRenderer
{
    public PrintPoint RenderBox(BoxLayout boxLayout, Graphics graphics, LayoutConstraints constraints)
    {
        var renderHeight = boxLayout.Height?.Min(constraints.Height) ?? PrintPoint.Zero;
        if (boxLayout.FillColor == null || renderHeight.Points == 0) return renderHeight;
        
        var renderWidth = boxLayout.Width?.Min(constraints.Width) ?? constraints.Width;

        graphics.FillRectangle(
            constraints.X.Points,
            constraints.Y.Points,
            renderWidth.Points,
            renderHeight.Points,
            boxLayout.FillColor.Value.ToVectSharpColor()
        );
        return renderHeight;
    }
}
using AquaVitae.Layouts;
using AquaVitae.Layouts.Types;
using VectSharp;

namespace AquaVitae.Render.Vector;

public sealed class VectorCanvasRenderer(VectorElementRenderer elementRenderer)
{
    public PrintPoint RenderCanvas(CanvasLayout canvasLayout, Graphics graphics, PrintPoint x, PrintPoint y)
    {
        var canvas = new Graphics();

        var totalWidth = 0f;
        var totalHeight = 0f;
        foreach (var positionedElement in canvasLayout.Elements)
        {
            canvas.Save();
            canvas.Translate(new Point(positionedElement.X.Points, positionedElement.Y.Points));
            elementRenderer.RenderElement(
                positionedElement.Element,
                canvas,
                x + positionedElement.X,
                y + positionedElement.Y,
                positionedElement.Width,
                positionedElement.Height
            );
            canvas.Restore();
            totalWidth = MathF.Max(totalWidth, positionedElement.X.Points + positionedElement.Width.Points);
            totalHeight = MathF.Max(totalHeight, positionedElement.Y.Points + positionedElement.Height.Points);
        }

        canvas.Crop(new Rectangle(
            0,
            0,
            canvasLayout.Width?.Points ?? totalWidth,
            canvasLayout.Height?.Points ?? totalHeight
        ));
        
        graphics.DrawGraphics(0, 0, canvas);

        return canvasLayout.Height ?? PrintPoint.Zero;
    }
}
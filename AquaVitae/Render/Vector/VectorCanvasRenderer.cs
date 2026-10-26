using AquaVitae.Layouts;
using AquaVitae.Layouts.Types;
using VectSharp;

namespace AquaVitae.Render.Vector;

public sealed class VectorCanvasRenderer(VectorElementRenderer elementRenderer)
{
    public PrintPoint RenderCanvas(CanvasLayout canvasLayout, Graphics graphics, LayoutConstraints constraints)
    {
        var canvas = new Graphics();

        var totalWidth = PrintPoint.Zero;
        var totalHeight = PrintPoint.Zero;
        foreach (var positionedElement in canvasLayout.Elements)
        {
            canvas.Save();
            canvas.Translate(new Point(positionedElement.X.Points, positionedElement.Y.Points));
            elementRenderer.RenderElement(
                positionedElement.Element,
                canvas,
                new LayoutConstraints(
                    constraints.X + positionedElement.X, 
                    constraints.Y + positionedElement.Y, 
                    positionedElement.Width,
                    positionedElement.Height
                )
            );
            canvas.Restore();
            totalWidth = totalWidth.Max(positionedElement.X + positionedElement.Width);
            totalHeight = totalHeight.Max(positionedElement.Y + positionedElement.Height);
        }

        canvas.Crop(new Rectangle(
            0,
            0,
            canvasLayout.Width?.Points ?? totalWidth.Points,
            canvasLayout.Height?.Points ?? totalHeight.Points
        ));
        
        graphics.DrawGraphics(constraints.X.Points, constraints.Y.Points, canvas);

        return canvasLayout.Height ?? PrintPoint.Zero;
    }
}
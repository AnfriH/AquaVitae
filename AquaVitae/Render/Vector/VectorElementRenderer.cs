using AquaVitae.Layouts;
using AquaVitae.Layouts.Abstractions;
using AquaVitae.Layouts.Types;
using VectSharp;

namespace AquaVitae.Render.Vector;

public sealed class VectorElementRenderer(
    VectorTextBoxRenderer vectorTextBoxRenderer,
    VectorBoxRenderer vectorBoxRenderer
)
{
    /// <summary>
    /// Renders an element to the provided graphics pane. 
    /// </summary>
    /// <param name="element">The element to be rendered</param>
    /// <param name="graphics">The graphics pane to render to</param>
    /// <param name="width">The target width of the graphics pane</param>
    /// <param name="height">
    /// The target height of the graphics pane.
    /// When this is null, the height will be determined by the element.
    /// </param>
    /// <returns>The actual height of the element rendered to the graphics</returns>
    /// <remarks>
    /// The width and height provided do not limit the actual size of the rendered output.
    /// Consumers of this method may choose to call <see cref="Graphics.Crop(Point, Size)"/>
    /// to crop the rendered output to the specified dimensions.
    /// </remarks>
    public PrintPoint RenderElement(
        ElementLayoutBase element,
        Graphics graphics,
        PrintPoint width,
        PrintPoint? height
    )
    {
        return element switch
        {
            GridLayout gridLayout => new VectorGridRenderer(this, gridLayout, width, graphics).RenderGrid(),
            TextBoxLayout textBoxLayout => vectorTextBoxRenderer.RenderTextBox(textBoxLayout, graphics, width),
            BoxLayout boxLayout => vectorBoxRenderer.RenderBox(boxLayout, graphics, width, height),
            CanvasLayout canvasLayout => throw new NotImplementedException(),
        };
    }
}

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
using AquaVitae.Layouts;
using AquaVitae.Layouts.Abstractions;
using AquaVitae.Layouts.Types;
using VectSharp;

namespace AquaVitae.Render.Vector;

public sealed class VectorElementRenderer
{
    private readonly VectorTextBoxRenderer _textBoxRenderer;
    private readonly VectorBoxRenderer _boxRenderer = new();
    private readonly VectorCanvasRenderer _canvasRenderer;

    public VectorElementRenderer(VectorStyleRenderer styleRenderer)
    {
        _textBoxRenderer = new VectorTextBoxRenderer(new VectorParagraphRenderer(styleRenderer), styleRenderer);
        _canvasRenderer = new VectorCanvasRenderer(this);
    }

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
            TextBoxLayout textBoxLayout => _textBoxRenderer.RenderTextBox(textBoxLayout, graphics, width),
            BoxLayout boxLayout => _boxRenderer.RenderBox(boxLayout, graphics, width, height),
            CanvasLayout canvasLayout => _canvasRenderer.RenderCanvas(canvasLayout, graphics),
        };
    }
}
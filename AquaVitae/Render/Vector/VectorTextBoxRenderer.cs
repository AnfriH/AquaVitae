using AquaVitae.Layouts;
using VectSharp;

namespace AquaVitae.Render.Vector;

public class VectorTextBoxRenderer(
    VectorDocumentRenderer documentRenderer,
    DocumentLayout documentLayout
)
{
    public void RenderTextBox(TextBoxLayout textBoxLayout, Graphics graphics)
    {
        if (textBoxLayout.FillColor.HasValue)
        {
            var fillColor = textBoxLayout.FillColor.Value;
            
            graphics.FillRectangle(
                textBoxLayout.X.Points,
                textBoxLayout.Y.Points,
                textBoxLayout.Width.Points,
                textBoxLayout.Height.Points,
                fillColor.ToVectSharpColor()
            );
        }
    }
}
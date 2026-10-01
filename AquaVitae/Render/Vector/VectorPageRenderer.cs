using AquaVitae.Layouts;
using VectSharp;

namespace AquaVitae.Render.Vector;

public class VectorPageRenderer(
    VectorDocumentRenderer documentRenderer,
    DocumentLayout documentLayout
)
{
    private VectorStyleRenderer VectorStyleRenderer => field ??= new VectorStyleRenderer(documentLayout.Styles);
    private VectorTextBoxRenderer TextBoxRenderer => field ??= new VectorTextBoxRenderer(
        new VectorParagraphRenderer(VectorStyleRenderer),
        VectorStyleRenderer
    );
    
    public Page RenderPage(PageLayout pageLayout)
    {
        var (width, height) = pageLayout.PageSize;
        
        var page = new Page(width.Points, height.Points);

        foreach (var elementLayout in pageLayout.PageElements)
        {
            switch (elementLayout)
            {
                case TextBoxLayout textBoxLayout:
                    TextBoxRenderer.RenderTextBox(textBoxLayout, page.Graphics);
                    break;
            }
        }
        
        if (pageLayout.FillColor.HasValue)
        {
            page.Background = pageLayout.FillColor.Value.ToVectSharpColor();
        }
        
        return page;
    }
}
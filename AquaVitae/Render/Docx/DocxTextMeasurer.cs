using AquaVitae.Layouts.Abstractions;
using AquaVitae.Render.Vector;
using Markdig.Syntax;
using VectSharp;
using VectSharp.Markdown;
using Margins = AquaVitae.Layouts.Types.Margins;

namespace AquaVitae.Render.Docx;

/// <summary>
/// Measures a layout's paragraphs and distributes them across pages.
/// </summary>
/// <remarks>
/// Technically, this type deals with rendering of a vector document.
/// However, it only does this to facilitate measuring text.
/// </remarks>
public sealed class DocxTextMeasurer : VectorParagraphRenderer
{
    private readonly IReadOnlyList<SvgPage> _svgPages;
    private readonly Margins _margins;
    
    private readonly Dictionary<Block, ParagraphLayoutBase> _owningBlocks = new(ReferenceEqualityComparer.Instance);
    private readonly List<List<ParagraphLayoutBase>> _paragraphLayoutsByPage;
    private int _pageIndex;
    
    public DocxTextMeasurer(
        VectorStyleRenderer styleRenderer,
        IReadOnlyList<SvgPage> svgPages,
        Margins margins
    ) : base(styleRenderer)
    {
        _svgPages = svgPages;
        _margins = margins;
        _paragraphLayoutsByPage = [];
    }

    public static List<List<ParagraphLayoutBase>> GetParagraphLayoutsByPage(
        IEnumerable<ParagraphLayoutBase> paragraphs,
        VectorStyleRenderer styleRenderer,
        IReadOnlyList<SvgPage> svgPages,
        Margins margins
    )
    {
        var measurer = new DocxTextMeasurer(styleRenderer, svgPages, margins);
        var markdownRenderer = new VectorMarkdownRenderer(measurer, styleRenderer);

        var markdownDocument = markdownRenderer.RenderToDocument(paragraphs);
        _ = measurer.Render(markdownDocument, out _, out _);
        
        return measurer._paragraphLayoutsByPage;
    }

    public override void StyleParagraph(Block block, ParagraphLayoutBase layout)
    {
        base.StyleParagraph(block, layout);
        _owningBlocks[block] = layout;
    }

    protected override void OnPageStarted(ref MarkdownContext context, ref Graphics pageGraphics, Page page)
    {
        base.OnPageStarted(ref context, ref pageGraphics, page);
        if (_pageIndex >= _svgPages.Count) throw new OutOfPagesException();
        
        var svgPage = _svgPages[_pageIndex];
        
        var width = svgPage.Width - _margins.Left - _margins.Right;
        var height = svgPage.Height - _margins.Top - _margins.Bottom;
        
        if (width <= 0 || height <= 0) throw new InvalidOperationException(
            "Margins are too large for the provided page dimensions."
        );
        
        PageSize = new Size(width.Points, height.Points);
        _paragraphLayoutsByPage.Add([]);
    }

    protected override void OnPageFinished(ref MarkdownContext context, ref Graphics pageGraphics, Page page)
    {
        base.OnPageFinished(ref context, ref pageGraphics, page);
        _pageIndex++;
    }

    protected override void OnBlockRendered(ref MarkdownContext context, ref Graphics graphics, Block block)
    {
        base.OnBlockRendered(ref context, ref graphics, block);
        if (_owningBlocks.TryGetValue(block, out var layout))
        {
            _paragraphLayoutsByPage[_pageIndex].Add(layout);
        }
    }
}

public sealed class OutOfPagesException() : Exception(
    "The provided layout exceeded the number of svg pages provided."
);
using AquaVitae.Common;
using AquaVitae.Layouts.Abstractions;

namespace AquaVitae.Layouts;

public sealed class DocumentLayout : LayoutBase
{
    private OptionalList<PageLayout> _pages;
    public IReadOnlyList<PageLayout> Pages => _pages.AsReadOnly();
    public StyleLayout Styles { get; } = new();

    public void AddPage(PageLayout page)
    {
        _pages.Add(page);
    }

    public void CollectParagraphs(Action<ParagraphLayoutBase> callback)
    {
        foreach (var pageLayout in Pages)
        {
            pageLayout.Background?.CollectParagraphs(callback);
            pageLayout.Grid?.CollectParagraphs(callback);
            pageLayout.Foreground?.CollectParagraphs(callback);
        }
    }
}
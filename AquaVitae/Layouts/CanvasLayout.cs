using AquaVitae.Common;
using AquaVitae.Layouts.Abstractions;
using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public sealed class CanvasLayout : ElementLayoutBase
{
    private OptionalList<PositionedElement> _elements = [];
    public IReadOnlyList<PositionedElement> Elements => _elements.AsReadOnly();

    public void AddElement(
        ElementLayoutBase element,
        PrintPoint x,
        PrintPoint y,
        PrintPoint width = default,
        PrintPoint height = default
    )
    {
        _elements.Add(new PositionedElement(x, y, width, height, element));
    }
    
    public override void CollectParagraphs(Action<ParagraphLayoutBase> callback)
    {
        foreach (var element in _elements)
        {
            element.Element.CollectParagraphs(callback);
        }
    }
}

public sealed record PositionedElement(
    PrintPoint X,
    PrintPoint Y,
    PrintPoint Width,
    PrintPoint Height,
    ElementLayoutBase Element
);
using AquaVitae.Common;
using AquaVitae.Layouts.Abstractions;
using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public sealed class CanvasLayout : ElementLayoutBase
{
    private OptionalList<PositionedElement> _elements = [];
    public IReadOnlyList<PositionedElement> Elements => _elements.AsReadOnly();

    public void AddElement(ElementLayoutBase element, PrintPoint x, PrintPoint y)
    {
        _elements.Add(new PositionedElement(x, y, element));
    }
    
    public override void CollectParagraphs(Action<ParagraphLayoutBase> callback)
    {
        foreach (var element in _elements)
        {
            element.Element.CollectParagraphs(callback);
        }
    }
}

public readonly record struct PositionedElement(PrintPoint X, PrintPoint Y, ElementLayoutBase Element);
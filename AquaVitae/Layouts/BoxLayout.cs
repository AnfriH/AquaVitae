using AquaVitae.Layouts.Abstractions;
using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public sealed class BoxLayout : ElementLayoutBase
{
    public PrintPoint? Width { get; init; }
    public PrintPoint? Height { get; init; }
    public Color? FillColor { get; init; }
    
    public override void CollectParagraphs(Action<ParagraphLayoutBase> callback)
    {
    }
}
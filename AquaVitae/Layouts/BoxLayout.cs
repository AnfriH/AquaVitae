using AquaVitae.Layouts.Abstractions;
using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public sealed class BoxLayout(PrintPoint width, PrintPoint height, Color? fillColor = null) : ElementLayoutBase
{
    public PrintPoint Width => width;
    public PrintPoint Height => height;
    public Color? FillColor => fillColor;
    
    public override void CollectParagraphs(Action<ParagraphLayoutBase> callback)
    {
    }
}
using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public closed class PageElementLayoutBase : LayoutBase
{
    PrintPoint X { get; }
    PrintPoint Y { get; }
    PrintPoint Width { get; }
    PrintPoint Height { get; }
}
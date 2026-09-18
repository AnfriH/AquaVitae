using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public interface IPageElementLayout : ILayout
{
    PrintPoint X { get; }
    PrintPoint Y { get; }
    PrintPoint Width { get; }
    PrintPoint Height { get; }
}
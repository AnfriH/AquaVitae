using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts;

public interface IPageElementLayout : ILayout
{
    Length X { get; }
    Length Y { get; }
    Length Width { get; }
    Length Height { get; }
}
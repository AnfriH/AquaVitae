using System.Xml;
using AquaVitae.Layouts.Types;

namespace AquaVitae.Render.Vector;

public sealed class SvgDocument(SvgPage[] pages)
{
    public IReadOnlyList<SvgPage> Pages => pages;
}

public readonly record struct SvgPage(PrintPoint Width, PrintPoint Height, XmlDocument Svg);
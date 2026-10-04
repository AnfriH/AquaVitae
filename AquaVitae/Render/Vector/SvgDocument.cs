using System.Xml;
using AquaVitae.Layouts.Types;

namespace AquaVitae.Render.Vector;

public sealed class SvgDocument(List<SvgPage> pages)
{
    public IReadOnlyList<SvgPage> Pages => pages;
}

public sealed record SvgPage(
    PrintPoint Width,
    PrintPoint Height,
    XmlDocument Document,
    IReadOnlyList<LinkPosition> Hyperlinks
);
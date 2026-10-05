using AquaVitae.Common;

namespace AquaVitae.Render.Vector;

public sealed class VectorHyperlinkRenderer
{
    private OptionalDictionary<string, string> _hyperlinks;
    private OptionalList<LinkPosition> _linkPositions;
    
    // We leak the internal dictionary object because VectSharp is too restrictive
    // with the current API.
    public Dictionary<string, string>? Hyperlinks => _hyperlinks.BackingDictionary;
    public IReadOnlyList<LinkPosition> LinkPositions => _linkPositions.AsReadOnly();

    public void AddHyperlinks(IDictionary<string, string> hyperlinks)
    {
        foreach (var (id, uri) in hyperlinks)
        {
            _hyperlinks.Add(id, uri);
        }
    }

    public void AddLinkBounds(IReadOnlyList<LinkPosition> paragraphRendererLinks)
    {
        _linkPositions.AddRange(paragraphRendererLinks);
    }
}
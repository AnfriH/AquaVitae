using AquaVitae.Common;

namespace AquaVitae.Render.Vector;

public sealed class VectorHyperlinkRenderer
{
    private OptionalDictionary<string, string> _hyperlinks;
    
    // We leak the internal dictionary object because VectSharp is too restrictive
    // with the current API.
    public Dictionary<string, string>? Hyperlinks => _hyperlinks.BackingDictionary;

    public void AddHyperlinks(IDictionary<string, string> hyperlinks)
    {
        foreach (var (id, uri) in hyperlinks)
        {
            _hyperlinks.Add(id, uri);
        }
    }
}
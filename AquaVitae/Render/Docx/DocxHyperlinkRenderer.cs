using DocumentFormat.OpenXml.Packaging;

namespace AquaVitae.Render.Docx;

public sealed class DocxHyperlinkRenderer(MainDocumentPart documentPart)
{
    private readonly Dictionary<Uri, string> _relationships = new();

    public string AddHyperlink(Uri uri)
    {
        if (_relationships.TryGetValue(uri, out var relId)) return relId;
        
        var rel = documentPart.AddHyperlinkRelationship(uri, true);
        _relationships.Add(uri, rel.Id);
        
        return rel.Id;
    }
}
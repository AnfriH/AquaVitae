using AquaVitae.Layouts;

namespace AquaVitae.Rendering;

public interface IDocumentRenderer
{
    Task RenderAsync(DocumentLayout layout, Stream output);
}
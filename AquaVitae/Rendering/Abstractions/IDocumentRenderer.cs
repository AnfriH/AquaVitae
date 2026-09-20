using AquaVitae.Layouts;

namespace AquaVitae.Rendering.Abstractions;

public interface IDocumentRenderer
{
    Task RenderDocumentAsync(DocumentLayout layout, Stream output);
}
using AquaVitae.Layouts;

namespace AquaVitae.Render.Abstractions;

public interface IDocumentRenderer
{
    Task RenderDocumentAsync(DocumentLayout layout, Stream output);
}
namespace AquaVitae.Render.Docx;

public sealed record DocxRendererSettings
{
    public bool IncludeTextColor { get; init; } = false;
}
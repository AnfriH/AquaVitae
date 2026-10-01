using AquaVitae.Common;

namespace AquaVitae.Layouts.Styles;

public sealed record FontStyle(
    string Name,
    string Id
) : IStyle
{
    public string? RegularFile { get; init; }
    public string? BoldFile { get; init; }
    public string? ItalicFile { get; init; }
    public string? BoldItalicFile { get; init; }
}
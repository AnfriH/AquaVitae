using AquaVitae.Common;

namespace AquaVitae.Layouts.Styles;

public sealed record FontStyle(
    string Name,
    string Id
) : IStyle
{
    public string? Regular { get; init; }
    public string? Bold { get; init; }
    public string? Italic { get; init; }
    public string? BoldItalic { get; init; }
}
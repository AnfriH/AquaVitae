using AquaVitae.Layouts.Types;

namespace AquaVitae.Layouts.Styles;

public sealed record VerticalListStyle(string Name, string Id) : IStyle<VerticalListLayout>
{
    public VerticalListType Type { get; init; }
    public PrintPoint FirstIndentation { get; init; }
    public PrintPoint FollowingIndentation { get; init; }
}

public enum VerticalListType 
{
    Unordered,
    Ordered
}
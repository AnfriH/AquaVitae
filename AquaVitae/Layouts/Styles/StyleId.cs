using System.Diagnostics.CodeAnalysis;

namespace AquaVitae.Layouts.Styles;

public readonly record struct StyleId<TLayout>(string? Value) where TLayout : LayoutBase
{
    [MemberNotNullWhen(false, nameof(Value))]
    public bool IsNone => Value == null;
}
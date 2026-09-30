using System.Diagnostics.CodeAnalysis;

namespace AquaVitae.Layouts.Styles;

/// <summary>
/// A unique identifier for a document style element
/// </summary>
public readonly record struct StyleId<TStyle>(string? Value) where TStyle : IStyle
{
    [MemberNotNullWhen(false, nameof(Value))]
    public bool IsNone => Value == null;
}
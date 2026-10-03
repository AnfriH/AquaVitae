using AquaVitae.Layouts.Abstractions;
using AquaVitae.Layouts.Styles;

namespace AquaVitae.Layouts;

public sealed class RunLayout(string text, StyleId<RunStyle> style) : RunLayoutBase(text, style);
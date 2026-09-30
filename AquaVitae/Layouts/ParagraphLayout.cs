using AquaVitae.Common;
using AquaVitae.Layouts.Styles;

namespace AquaVitae.Layouts;

public sealed class ParagraphLayout(StyleId<ParagraphStyle> style) : ParagraphLayoutBase
{
    public StyleId<ParagraphStyle> Style => style;
    private OptionalList<RunLayoutBase> _runs;
    public IReadOnlyList<RunLayoutBase> Runs => _runs.AsReadOnly();
    public void AddRun(RunLayoutBase run) => _runs.Add(run);
}
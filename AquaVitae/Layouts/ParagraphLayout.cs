using AquaVitae.Common;
using AquaVitae.Layouts.Styles;

namespace AquaVitae.Layouts;

public sealed class ParagraphLayout : ParagraphLayoutBase
{
    public StyleId<ParagraphLayout> Style { get; init; }
    private OptionalList<RunLayout> _runs;
    public IReadOnlyList<RunLayout> Runs => _runs.AsReadOnly();
    public void AddRun(RunLayout run) => _runs.Add(run);
}
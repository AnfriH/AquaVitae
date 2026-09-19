using AquaVitae.Common;

namespace AquaVitae.Layouts;

public sealed class ParagraphLayout : ILayout
{
    private OptionalList<RunLayout> _runs;
    public IReadOnlyList<RunLayout> Runs => _runs.AsReadOnly();
    public void AddRun(RunLayout run) => _runs.Add(run);
}
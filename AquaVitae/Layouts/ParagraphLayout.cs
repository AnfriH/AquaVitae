using AquaVitae.Common;

namespace AquaVitae.Layouts;

public sealed class ParagraphLayout : ILayout
{
    public string? StyleId { get; init; }
    private OptionalList<RunLayout> _runs;
    public IReadOnlyList<RunLayout> Runs => _runs.AsReadOnly();
    public void AddRun(RunLayout run) => _runs.Add(run);
}
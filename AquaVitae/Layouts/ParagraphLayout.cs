namespace AquaVitae.Layouts;

public sealed class ParagraphLayout : ILayout
{
    private readonly List<RunLayout> _runs = [];
    public IReadOnlyList<RunLayout> Runs => _runs;
    
    public void AddRun(RunLayout run) => _runs.Add(run);
}
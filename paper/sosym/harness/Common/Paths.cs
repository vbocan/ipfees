namespace Harness.Common;

/// <summary>
/// Where the harness finds its inputs and writes its results. Everything is relative to the
/// evaluation folder so the harness runs the same from any checkout of upt-research.
/// </summary>
public static class Paths
{
    public static string Evaluation { get; } = FindEvaluationRoot();

    public static string Work => Path.Combine(Evaluation, "work");
    public static string Results => Path.Combine(Evaluation, "results");

    public static string ResultsFor(string experiment)
    {
        var dir = Path.Combine(Results, experiment);
        Directory.CreateDirectory(dir);
        return dir;
    }

    private static string FindEvaluationRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "harness", "Harness.csproj"))) return dir.FullName;
            dir = dir.Parent;
        }
        throw new InvalidOperationException("Could not locate the evaluation folder above " + AppContext.BaseDirectory);
    }
}

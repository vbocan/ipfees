using System.Diagnostics;
using System.Text.Json;

namespace Harness.Common;

/// <summary>
/// Runs one measurement in a fresh child process. The engine's verifiers take no
/// cancellation token, so a run abandoned at the budget keeps a thread spinning; isolating each
/// run in its own process, and killing it after budget plus grace, keeps an abandoned run from
/// slowing the ones measured after it.
/// </summary>
public static class Worker
{
    public static JsonElement? Run(TimeSpan kill, params string[] args)
    {
        var self = typeof(Worker).Assembly.Location;
        var host = Environment.ProcessPath!;
        var psi = new ProcessStartInfo(host)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        if (Path.GetFileNameWithoutExtension(host).Equals("dotnet", StringComparison.OrdinalIgnoreCase)) psi.ArgumentList.Add(self);
        foreach (var a in args) psi.ArgumentList.Add(a);

        using var process = Process.Start(psi)!;
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(kill))
        {
            try { process.Kill(entireProcessTree: true); } catch { }
            process.WaitForExit();
            return null;
        }
        var line = stdout.Result.Split('\n', StringSplitOptions.RemoveEmptyEntries).LastOrDefault(l => l.TrimStart().StartsWith("{"));
        if (line is null)
        {
            Console.Error.WriteLine($"worker {string.Join(" ", args)} produced no result: {stderr.Result}");
            return null;
        }
        return JsonDocument.Parse(line).RootElement.Clone();
    }

    /// <summary>Print the worker's result and leave at once, taking any abandoned thread with it.</summary>
    public static int Finish(object result)
    {
        Console.Out.WriteLine(JsonSerializer.Serialize(result));
        Console.Out.Flush();
        Environment.Exit(0);
        return 0;
    }
}

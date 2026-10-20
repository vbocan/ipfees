using Harness.Common;

namespace Harness.E2;

public static class CountMutants
{
    public static int Run(string[] args)
    {
        var all = PackagedCorpus.Codes.SelectMany(c => Mutator.Enumerate(c, PackagedCorpus.Source(c))).ToList();
        foreach (var g in all.GroupBy(m => m.Operator)) Console.WriteLine($"{g.Key}: {g.Count()} mutants in {g.Select(m => m.Code).Distinct().Count()} schedules");
        Console.WriteLine($"total {all.Count}");
        foreach (var m in all.Where(m => args.Contains(m.Code)).Take(400)) Console.WriteLine($"{m.Id} | {m.Fee} | L{m.Line} | {m.Original} => {m.Mutated} | {m.Detail}");
        return 0;
    }
}

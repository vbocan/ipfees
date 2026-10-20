namespace Harness;

public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("usage: Harness <e2|e3|e2-count> [codes] [options]");
            return 2;
        }

        var rest = args.Skip(1).ToArray();
        return args[0] switch
        {
            "e2-count" => Harness.E2.CountMutants.Run(rest),
            "e2" => Harness.E2.MutationStudy.Run(rest),
            "e2-baseline" => Harness.E2.MutationStudy.Baseline(rest),
            "e2-worker" => Harness.E2.MutationStudy.Work(rest),
            "e3" => Harness.E3.VerificationCost.Run(rest),
            "e3-worker" => Harness.E3.VerificationCost.Work(rest),
            _ => Unknown(args[0])
        };
    }

    private static int Unknown(string command)
    {
        Console.Error.WriteLine($"unknown command '{command}'");
        return 2;
    }
}

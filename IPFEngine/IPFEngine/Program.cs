using IPFees.Calculator;
using IPFees.Evaluator;
using Pastel;
using System.Drawing;
using System.Linq;

string text = File.ReadAllText(@"..\..\..\us_fees.ipf");

var calc = new IPFCalculator(text);
var CalcErrors = calc.GetErrors();

if (CalcErrors.Count() == 0)
{
    Console.WriteLine("PARSED VARIABLES ===============================================".Pastel(ConsoleColor.Yellow));
    foreach (var v in calc.GetVariables())
    {
        Console.WriteLine(v);
    }

    Console.WriteLine();
    Console.WriteLine("PARSED FEES: ===================================================".Pastel(ConsoleColor.Yellow));
    foreach (var f in calc.GetFees())
    {
        Console.WriteLine(f);
    }
}
else
{
    Console.WriteLine("Errors detected:");
    foreach (var e in calc.GetErrors())
    {
        Console.WriteLine(e);
    }
    return;
}

Console.WriteLine();
Console.WriteLine("FEE COMPUTATION: ========================================".Pastel(ConsoleColor.Yellow));
var vars = new IPFValue[] {
    new IPFValueString("EntityType", "NormalEntity"),
    new IPFValueString("SituationType", "PreparedISA"),
    new IPFValueNumber("SheetCount", 120),
    new IPFValueNumber("ClaimCount", 7)
};

Console.WriteLine("Input variables:".Pastel(ConsoleColor.White));
foreach (var v in vars)
{
    Console.WriteLine(v.ToString().Pastel(ConsoleColor.Cyan));
}
Console.WriteLine();

var (TotalAmount, CalculationSteps) = calc.Compute(vars);

Console.WriteLine("Total amount (for all fees): {0}".Pastel(Color.White).PastelBg(Color.Red), TotalAmount);

Console.WriteLine("CALCULATION STEPS: ======================================".Pastel(ConsoleColor.Yellow));
foreach (var s in CalculationSteps)
{
    Console.WriteLine(s);
}

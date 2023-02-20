using IPFees.Calculator;
using IPFees.Evaluator;
using Pastel;
using System.Drawing;


string text = File.ReadAllText(@"..\..\..\us_fees.ipf");

var calc = new IPFCalculator();
calc.Parse(text);
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

var (TotalMandatoryAmount, TotalOptionalAMount, CalculationSteps) = calc.Compute(vars);

Console.WriteLine("Total mandatory amount: {0}".Pastel(Color.White).PastelBg(Color.DarkRed), TotalMandatoryAmount);
Console.WriteLine("Total optional amount: {0}".Pastel(Color.White).PastelBg(Color.DarkRed), TotalOptionalAMount);
Console.WriteLine("Grand Total: {0}".Pastel(Color.White).PastelBg(Color.Red), TotalMandatoryAmount + TotalOptionalAMount);

Console.WriteLine("CALCULATION STEPS: ======================================".Pastel(ConsoleColor.Yellow));
foreach (var s in CalculationSteps)
{
    Console.WriteLine(s);
}

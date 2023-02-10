using IPFEngine.Evaluator;
using IPFEngine.Parser;
using Pastel;
using System.Drawing;
using System.Linq;

string text = File.ReadAllText(@"..\..\..\us_fees.ipf");
var p = new IPFParser(text);
var (Variables, Fees) = p.Parse();

Console.WriteLine("PARSED VARIABLES ===============================================".Pastel(ConsoleColor.Yellow));
foreach (var v in Variables)
{
    Console.WriteLine(v);
}

Console.WriteLine();
Console.WriteLine("PARSED FEES: ===================================================".Pastel(ConsoleColor.Yellow));
foreach (var f in Fees)
{
    Console.WriteLine(f);
}
Console.WriteLine();
Console.WriteLine("SEMANTIC CHECK: =========================================".Pastel(ConsoleColor.Yellow));
var ck = IPFSemanticChecker.Check(Variables, Fees);
if (!ck.Any())
{
    Console.WriteLine("No errors detected");
}
else
{
    Console.WriteLine("Errors detected:");
    foreach (var e in ck)
    {
        Console.WriteLine(e);
    }
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
foreach(var v in vars)
{
    Console.WriteLine(v.ToString().Pastel(ConsoleColor.Cyan));
}
Console.WriteLine();

int TotalAmount = 0;
foreach (var fee in Fees)
{
    Console.WriteLine("COMPUTING FEE [{0}]".Pastel(ConsoleColor.White), fee.Name);
    int Amount = 0;
    Console.WriteLine("Amount is initially {0}", Amount);
    foreach (IPFFeeCase fc in fee.Cases)
    {
        var case_cond = (fc.Condition.Count() == 0) ? true : IPFEvaluator.EvaluateLogic(fc.Condition.ToArray(), vars);
        if (!case_cond)
        {
            Console.WriteLine("Condition [{0}] is FALSE, skipping", string.Join(" ", fc.Condition));
            continue;
        }
        if(fc.Condition.Count() >0) Console.WriteLine("Condition [{0}] is TRUE, proceeding with evaluating individual expressions", string.Join(" ", fc.Condition));
        foreach (var b in fc.Yields)
        {
            var cond_b = IPFEvaluator.EvaluateLogic(b.Condition.ToArray(), vars);
            var val_b = IPFEvaluator.EvaluateExpression(b.Values.ToArray(), vars);
            Console.WriteLine("Condition: [{0}] is [{1}]", string.Join(" ", b.Condition), cond_b);
            if (cond_b)
            {
                Amount += val_b;
                Console.WriteLine("After evaluating expression [{0}], the amount is {1}", string.Join(" ", b.Values), Amount);
            }
        }
    }
    Console.WriteLine("Finally, the amount for fee {0} is {1}".Pastel(ConsoleColor.White), fee.Name, Amount);
    Console.WriteLine();
    TotalAmount += Amount;
}

Console.WriteLine("Total amount (for all fees): {0}".Pastel(Color.White).PastelBg(Color.Red), TotalAmount);

using IPFEngine.Evaluator;
using IPFEngine.Parser;
using System.Collections;

string text = File.ReadAllText(@"..\..\..\us_fees.ipf");
var p = new IPFParser(text);
var (Variables, Fees) = p.Parse();

Console.WriteLine("VARIABLES ===============================================");
foreach(var v in Variables)
{
    Console.WriteLine(v);
}

Console.WriteLine("FEES: ===================================================");
foreach (var f in Fees)
{
    Console.WriteLine(f);
}

Console.WriteLine("SEMANTIC CHECK: =========================================");
var ck = IPFSemanticChecker.Check(Variables, Fees);
if(ck.Count() == 0)
{
    Console.WriteLine("No errors detected.");
}else
{
    Console.WriteLine("Errors detected:");
    foreach(var e in ck)
    {
        Console.WriteLine(e);
    }
}

var vars = new Dictionary<string, string>();
vars["A"] = "10";
vars["B"] = "5";
vars["C"] = "45";
Console.WriteLine("EVALUATION: =============================================");
var tokens = "( B + 2 ) * C - 11 * ( B + 2 * A )".Split(new char[] { ' ' }, StringSplitOptions.None);
var ev = IPFEvaluator.EvaluateTokens(tokens, vars);
Console.WriteLine(ev);
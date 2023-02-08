using IPFEngine.Evaluator;
using IPFEngine.Parser;

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
if(!ck.Any())
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

var vars = new Dictionary<string, string>
{
    ["A"] = "10",
    ["B"] = "15",
    ["C"] = "45"
};
Console.WriteLine("EVALUATION: =============================================");
var tokens = "( B + 2 ) * C - 11 * ( B + 2 * A )".Split(new char[] { ' ' }, StringSplitOptions.None);
var ev = IPFEvaluator.EvaluateExpression(tokens, vars);
Console.WriteLine(ev);
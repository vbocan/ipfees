using IPFEngine.Evaluator;
using IPFEngine.Parser;

string text = File.ReadAllText(@"..\..\..\us_fees.ipf");
var p = new IPFParser(text);
var (Variables, Fees) = p.Parse();

Console.WriteLine("VARIABLES ===============================================");
foreach (var v in Variables)
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
if (!ck.Any())
{
    Console.WriteLine("No errors detected.");
}
else
{
    Console.WriteLine("Errors detected:");
    foreach (var e in ck)
    {
        Console.WriteLine(e);
    }
}

var vars = new IPFValue[] {
    new IPFValueNumber("A", 6),
    new IPFValueNumber("B", 80),
    new IPFValueNumber("C", 30),
    new IPFValueString("EntityType", "NormalEntity1"),
    new IPFValueNumber("SheetCount", 101),
};
Console.WriteLine("ARITHMETIC EVALUATION: =============================================");
var tokens = "( B + 2 ) * C - 11 * ( B + 2 * A )".Split(new char[] { ' ' }, StringSplitOptions.None);
var ev = IPFEvaluator.EvaluateExpression(tokens, vars);
Console.WriteLine(ev);

Console.WriteLine("INEQUALITY EVALUATION ==============================================");
var tokens2 = "A EQUALS 60 / 10".Split(new char[] { ' ' }, StringSplitOptions.None);
var ev2 = IPFEvaluator.EvaluateLogic(tokens2, vars);
Console.WriteLine(ev2);

Console.WriteLine("STRING EVALUATION: =================================================");
var tokens3 = "EntityType EQUALS NormalEntity".Split(new char[] { ' ' }, StringSplitOptions.None);
var ev3 = IPFEvaluator.EvaluateLogic(tokens3, vars);
Console.WriteLine(ev3);

Console.WriteLine("LOGIC EVALUATION: =================================================");
var tokens4 = "SheetCount ABOVE 100 AND EntityType EQUALS NormalEntity".Split(new char[] { ' ' }, StringSplitOptions.None);
var ev4 = IPFEvaluator.EvaluateLogic(tokens4, vars);
Console.WriteLine(ev4);

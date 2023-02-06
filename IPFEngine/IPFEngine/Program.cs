using IPFEngine.Parser;
using IPFEngine.SemanticCheck;

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
var ck = IPFSemanticCheck.Check(Variables, Fees);
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
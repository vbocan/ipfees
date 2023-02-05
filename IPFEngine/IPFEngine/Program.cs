using IPFEngine.Parser;

string text = File.ReadAllText(@"..\..\..\us_fees.ipf");
var p = new IPFParser(text);
var (Variables, Fees) = p.Parse();

Console.WriteLine("VARIABLES:");
foreach(var v in Variables)
{
    Console.WriteLine(v);
}

Console.WriteLine("FEES:");
foreach (var f in Fees)
{
    Console.WriteLine(f);
}

using IPFEngine.Parser;

string text = File.ReadAllText(@"..\..\..\us_fees.ipf");
var p = new IPFParser(text);
var result = p.Parse();
foreach(var res in result)
{
    Console.WriteLine(res);
}

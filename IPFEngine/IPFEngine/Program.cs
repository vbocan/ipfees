using IPFEngine.Parser;

string[] ipf_data = File.ReadAllLines(@"..\..\..\us_fees.ipf");
var p = new IPFParser(ipf_data);
var result = p.Parse();
foreach(var res in result)
{
    Console.WriteLine(res);
}

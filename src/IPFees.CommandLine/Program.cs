using IPFees.Core.Data;
using IPFees.Core.Enum;
using IPFees.Core.Repository;
using IPFees.Evaluator;
using SharpCompress.Common;

string filePath = "..\\..\\..\\..\\..\\db-backups\\IPFeesDev.Fees.csv";
var dc = new DataContext("mongodb+srv://valerbocan:cxpAkYCALM15zC8j@ipfeescluster.ayqdiey.mongodb.net/IPFeesDev?retryWrites=true&w=majority");
var jr = new JurisdictionRepository(dc);

string contents = File.ReadAllText(filePath);

string[] lines = contents.Split('\n');

foreach (string line in lines)
{
    string Name = line[..2];
    string Description = line[3..];
    Console.WriteLine($"Processing {Name},{Description}");
    var id = await jr.AddJurisdictionAsync(Name);
    await jr.SetJurisdictionDescriptionAsync(id.Id, Description);
    await jr.SetJurisdictionAttorneyFeeLevelAsync(id.Id, AttorneyFeeLevel.Level1);
}

/*
string folderPath = "..\\..\\..\\..\\..\\assets\\TranslationFiles";

string[] txtFiles = Directory.GetFiles(folderPath, "*.txt");
var dc = new DataContext("mongodb+srv://valerbocan:cxpAkYCALM15zC8j@ipfeescluster.ayqdiey.mongodb.net/IPFeesDev?retryWrites=true&w=majority");
var jr = new FeeRepository(dc);

foreach (string filePath in txtFiles)
{
    string fileName = Path.GetFileName(filePath);
    Console.WriteLine($"Processing file: {fileName}");

    string contents = File.ReadAllText(filePath);
    var Name = ExtractName(contents);
    if (string.IsNullOrEmpty(Name)) throw new ApplicationException("Invalid filename!");
    var Description = ExtractDescription(contents);
    if (string.IsNullOrEmpty(Description)) throw new ApplicationException("Invalid description!");

    var res = await jr.AddFeeAsync(Name);
    if (!res.Success)
    {
        throw new ApplicationException($"Impossible to add fee {Name}!");
    }
    await jr.SetFeeDescriptionAsync(res.Id, Description);
    await jr.SetAttorneyFeeLevelAsync(res.Id, AttorneyFeeLevel.Level1);    
    await jr.SetReferencedModules(res.Id, new Guid[] { Guid.Parse("907f2be0-8028-4df8-a2a6-39ae971a44a0") });
    await jr.SetFeeSourceCodeAsync(res.Id, contents);
    Console.WriteLine();
}

string ExtractName(string contents)
{
    string[] lines = contents.Split('\n');

    foreach (string line in lines)
    {
        string trimmedLine = line.Trim();

        if (!string.IsNullOrEmpty(trimmedLine))
        {
            // # File name: PCT-AE-AGT
            if (trimmedLine.StartsWith("# File name:"))
            {
                return trimmedLine[13..].Trim();
            }
        }
    }
    return string.Empty;
}

string ExtractDescription(string contents)
{
    string[] lines = contents.Split('\n');

    foreach (string line in lines)
    {
        string trimmedLine = line.Trim();

        if (!string.IsNullOrEmpty(trimmedLine))
        {
            // # File content: Partner fees for PCT national phase: United Arab Emirates
            if (trimmedLine.StartsWith("# File content:"))
            {
                return trimmedLine[15..].Trim();
            }
        }
    }
    return string.Empty;
}*/
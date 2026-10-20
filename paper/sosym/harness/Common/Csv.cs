using System.Globalization;
using System.Text;

namespace Harness.Common;

/// <summary>Minimal RFC 4180 writer; values are formatted with the invariant culture.</summary>
public sealed class Csv : IDisposable
{
    private readonly StreamWriter writer;

    public Csv(string path, params string[] header)
    {
        writer = new StreamWriter(path, false, new UTF8Encoding(false)) { NewLine = "\n" };
        Row(header);
    }

    public void Row(params object?[] values)
    {
        writer.WriteLine(string.Join(",", values.Select(Format)));
    }

    public static string Format(object? value)
    {
        var s = value switch
        {
            null => "",
            decimal d => d.ToString(CultureInfo.InvariantCulture),
            double d => d.ToString("R", CultureInfo.InvariantCulture),
            float f => f.ToString("R", CultureInfo.InvariantCulture),
            bool b => b ? "true" : "false",
            IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString() ?? ""
        };
        return s.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0 ? "\"" + s.Replace("\"", "\"\"") + "\"" : s;
    }

    public void Dispose() => writer.Dispose();
}

// HMS.Communication.Infrastructure/Drivers/Analyzers/RocheCobasDriver.cs
using HMS.Communication.Abstractions;
using System.Text;

namespace HMS.Communication.Infrastructure.Drivers.Analyzers;

public sealed class RocheCobasDriver : IAnalyzerDriver
{
    public string Name => "Roche Cobas ASTM (bench)";

    public byte[] BuildSimpleResult(string accession, string testCode, string value, string unit,
                                    string deviceCode = "DEV", string? mode = null)
    {
        // Header brand now reflects the actual device/mode you pass in
        var brand = string.IsNullOrWhiteSpace(mode) ? "cobas" : mode; // e.g., "e411","c311","cobas"
        var sb = new StringBuilder();
        sb.Append($"H|\\^&|||HMS^COMM|||||{brand}||P|1\r");
        sb.Append("P|1\r");
        sb.Append($"O|1|{accession}|{accession}||^^^{testCode}^1||||||||||O\r");
        sb.Append($"R|1|^^^{testCode}^1|{value}|{unit}|N||F\r");
        sb.Append("L|1|N\r");
        return Encoding.ASCII.GetBytes(sb.ToString());
    }
}

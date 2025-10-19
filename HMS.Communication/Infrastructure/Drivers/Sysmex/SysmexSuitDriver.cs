// HMS.Communication.Infrastructure/Drivers/Analyzers/SysmexSuitDriver.cs
using HMS.Communication.Abstractions;
using System.Text;

namespace HMS.Communication.Infrastructure.Drivers.Analyzers;

public sealed class SysmexSuitDriver : IAnalyzerDriver
{
    public string Name => "Sysmex (bench via ASTM)";

    public byte[] BuildSimpleResult(string accession, string testCode, string value, string unit,
                                    string deviceCode = "DEV", string? mode = null)
    {
        // Use a brand hint in the header so you can see it in traces/UI
        var brand = "sysmex";
        var sb = new StringBuilder();
        sb.Append($"H|\\^&|||HMS^COMM|||||{brand}||P|1\r");
        sb.Append("P|1\r");
        sb.Append($"O|1|{accession}|{accession}||^^^{testCode}^1||||||||||O\r");
        sb.Append($"R|1|^^^{testCode}^1|{value}|{unit}|N||F\r");
        sb.Append("L|1|N\r");
        return Encoding.ASCII.GetBytes(sb.ToString());
    }
}

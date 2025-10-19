// HMS.Communication.Infrastructure/Drivers/Analyzers/FujiDryChemDriver.cs
using HMS.Communication.Abstractions;
using System.Text;

namespace HMS.Communication.Infrastructure.Drivers.Analyzers;

public sealed class FujiDryChemDriver : IAnalyzerDriver
{
    public string Name => "Fuji Dri-Chem (bench via ASTM)";

    public byte[] BuildSimpleResult(string accession, string testCode, string value, string unit,
                                    string deviceCode = "DEV", string? mode = null)
    {
        var brand = "fuji";
        var sb = new StringBuilder();
        sb.Append($"H|\\^&|||HMS^COMM|||||{brand}||P|1\r");
        sb.Append("P|1\r");
        sb.Append($"O|1|{accession}|{accession}||^^^{testCode}^1||||||||||O\r");
        sb.Append($"R|1|^^^{testCode}^1|{value}|{unit}|N||F\r");
        sb.Append("L|1|N\r");
        return Encoding.ASCII.GetBytes(sb.ToString());
    }
}

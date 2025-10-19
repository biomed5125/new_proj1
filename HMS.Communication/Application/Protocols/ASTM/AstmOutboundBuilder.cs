using System.Text;
using HMS.Communication.Abstractions;

namespace HMS.Communication.Application.Protocols.ASTM;

internal sealed class AstmOutboundBuilder
{
    private readonly string _delims;
    public AstmOutboundBuilder(string delims) => _delims = string.IsNullOrWhiteSpace(delims) ? "|^\\&" : delims;

    public string BuildRecords(OrderDownload order)
    {
        var sb = new StringBuilder();
        sb.Append("H|\\^&|||HOST|||||cobas||||P|1\r");
        sb.Append("P|1\r");
        int i = 1;
        foreach (var line in order.Lines)
        {
            sb.Append($"O|{i}|{order.Accession}|||{line.InstrumentCode}^{line.LisCode}||R|{order.Priority}\r");
            i++;
        }
        sb.Append("L|1|N\r");
        return sb.ToString();
    }
}

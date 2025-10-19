
namespace HMS.CommBench.Models
{
    public record MessageRow(
    long Id, DateTimeOffset At, string DeviceCode,
    string Direction, string Type, string? Accession,
    string? InstrumentCode, bool ChecksumOk, int Size, string BusinessNo);

}

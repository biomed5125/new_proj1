using HMS.SharedKernel.Ids;

namespace HMS.Api.Infrastructure.Ids;

public sealed class TimeBasedBusinessIdGenerator : IBusinessIdGenerator
{
    private static int Luhn(string digits)
    {
        int sum = 0, alt = 0;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int n = digits[i] - '0';
            if ((alt++ & 1) == 1) { n *= 2; if (n > 9) n -= 9; }
            sum += n;
        }
        return (10 - (sum % 10)) % 10;
    }

    public long Next(string seqName) => throw new NotSupportedException("No DB sequences in time-based generator.");

    public string NewMrn()
    {
        // 10-digit time-based core + Luhn; prefixed with P
        var baseEpoch = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var seconds = (long)(DateTime.UtcNow - baseEpoch).TotalSeconds; // fit ~10 digits for many years
        var rnd = Random.Shared.Next(0, 100); // 00..99
        var core = $"{seconds:D8}{rnd:D2}";
        return $"P{core}{Luhn(core)}";
    }

    public string NewAppointmentNo(DateTime utc) => $"AP{utc:yyyyMMddHHmmssfff}";
    public string NewEncounterNo(DateTime utc) => $"E{utc:yyyyMMddHHmmssfff}";
    public string NewLabOrderNo(DateTime utc) => $"LR{utc:yyyyMMddHHmmssfff}";
    // DICOM recommends <=16 chars for AccessionNumber; keep it short:
    public string NewAccessionNumber(DateTime utc) => $"S{utc:yyMMddHHmmssfff}".Substring(0, 16);
    public string NewInvoiceNo(DateTime utc) => $"INV{utc:yyyy}{utc:MMddHHmmss}";
}
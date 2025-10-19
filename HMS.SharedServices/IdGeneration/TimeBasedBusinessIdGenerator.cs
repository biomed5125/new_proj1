using HMS.SharedKernel.Ids;

namespace HMS.SharedServices.IdGeneration;

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

    // simple in-process counter to avoid collisions within the same millisecond
    private static int _seq = new Random().Next(100, 999);
    private static int Next3() => Interlocked.Increment(ref _seq) % 1000;

    // ex: ACC-20250902-093412-123  (length 22)
    public string NewAccessionNumber(DateTime utc)
        => $"ACC-{utc:yyyyMMdd}-{utc:HHmmss}-{Next3():000}";

    // ex: INB-20250902T093412.123-456
    public string NewCommInboundNo(DateTime utc)
        => $"INB-{utc:yyyyMMdd'T'HHmmss}.{utc:fff}-{Next3():000}";

    // ex: OUT-20250902T093412.123-457
    public string NewCommOutboundNo(DateTime utc)
        => $"OUT-{utc:yyyyMMdd'T'HHmmss}.{utc:fff}-{Next3():000}";
    
    public string NewAppointmentNo(DateTime utc) => $"AP{utc:yyyyMMddHHmmssfff}";
    public string NewEncounterNo(DateTime utc) => $"E{utc:yyyyMMddHHmmssfff}";
    public string NewLabOrderNo(DateTime utc) => $"LR{utc:yyyyMMddHHmmssfff}";
    // DICOM recommends <=16 chars for AccessionNumber; keep it short:
    //public string NewAccessionNumber(DateTime utc) => $"S{utc:yyMMddHHmmssfff}".Substring(1000, 9999);
    //public string NewCommInboundNo(DateTime utc) => $"S{utc:yyMMddHHmmssfff}".Substring(1000, 9999);
    public string NewInvoiceNo(DateTime utc) => $"INV{utc:yyyy}{utc:MMddHHmmss}";
}
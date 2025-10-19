using System.Text;

namespace HMS.CommBench.Core;

public static class AstmCodec
{
    //public const byte STX = 0x02, ETX = 0x03, ENQ = 0x05, ACK = 0x06, NAK = 0x15, EOT = 0x04, CR = 0x0D, LF = 0x0A;

    public const byte STX = 0x02;
    public const byte ETX = 0x03;
    public const byte ETB = 0x17;
    public const byte EOT = 0x04;
    public const byte ENQ = 0x05;
    public const byte ACK = 0x06;
    public const byte NAK = 0x15;
    public const byte CR = 0x0D;
    public const byte LF = 0x0A;

    public const int MaxFrame = 240; // body length (ASTM typical)

    public static byte[] MakeFrame(char seq, string payload)
    {
        var payloadBytes = Encoding.ASCII.GetBytes($"{seq}{payload}{(char)ETX}");
        byte lrc = 0;
        foreach (var b in payloadBytes) lrc ^= b;

        var lrcText = lrc.ToString("X2");
        var frame = new List<byte>(1 + payloadBytes.Length + 2 + 2);
        frame.Add(STX);
        frame.AddRange(payloadBytes);
        frame.AddRange(Encoding.ASCII.GetBytes(lrcText));
        frame.Add(CR); frame.Add(LF);
        return frame.ToArray();
    }

    public static bool TryParse(ReadOnlySpan<byte> data, out string frameText, out int consumed)
    {
        frameText = ""; consumed = 0;
        int stx = data.IndexOf(STX);
        if (stx < 0) return false;
        if (stx > 0) { consumed = stx; return false; }

        int pos = 1;
        int etxPos = -1;
        for (int i = pos; i < data.Length; i++)
        {
            if (data[i] == ETX) { etxPos = i; break; }
        }
        if (etxPos < 0 || etxPos + 3 + 2 > data.Length) return false; // need ETX + LRC(2) + CR LF

        // compute LRC over seq..ETX
        byte lrc = 0;
        for (int i = 1; i <= etxPos; i++) lrc ^= data[i];
        var lrcText = lrc.ToString("X2");

        var recLrcText = Encoding.ASCII.GetString(data.Slice(etxPos + 1, 2));
        if (!string.Equals(lrcText, recLrcText, StringComparison.OrdinalIgnoreCase)) { consumed = etxPos + 3; return false; }

        if (data[etxPos + 3] != LF) return false; // expect CR LF
        // payload excluding seq and ETX
        frameText = Encoding.ASCII.GetString(data.Slice(2, etxPos - 2)); // skip seq
        consumed = etxPos + 4; // STX..LF inclusive
        return true;
    }
}

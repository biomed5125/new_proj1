using System;

namespace HMS.Communication.Application.Protocols.ASTM;

public static class AstmChecksum
{
    public static string Lrc(ReadOnlySpan<byte> span)
    {
        byte lrc = 0;
        for (int i = 0; i < span.Length; i++) lrc ^= span[i];
        return lrc.ToString("X2");
    }
}

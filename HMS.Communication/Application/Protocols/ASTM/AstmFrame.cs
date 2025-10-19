// HMS.Communication/Application/Protocols/ASTM/AstmFrame.cs
using System;
using System.Text;

namespace HMS.Communication.Application.Protocols.ASTM;

public sealed class AstmFrame
{
    public byte Seq { get; init; }
    public string Text { get; init; } = "";
    public string Checksum { get; init; } = "";

    public byte[] Serialize()
    {
        // <STX><seq><text><ETX><cs><CR><LF>
        var body = $"{(char)AstmConstants.STX}{Seq}{Text}{(char)AstmConstants.ETX}";
        // compute LRC from AFTER STX (start at seq) through ETX inclusive
        var lrc = AstmChecksum.Lrc(Encoding.ASCII.GetBytes(body.Substring(1))); // <-- fixed
        var final = $"{body}{lrc}{(char)AstmConstants.CR}{(char)AstmConstants.LF}";
        return Encoding.ASCII.GetBytes(final);
    }
}

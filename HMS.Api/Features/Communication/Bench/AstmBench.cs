//// Features/Communication/Bench/AstmBench.cs
//namespace HMS.Api.Features.Communication.Bench;

//internal static class AstmBench
//{
//    // Turn "<STX>...<ETX><CR><LF>" into real control chars
//    public static string ReplaceTokens(string s)
//    {
//        if (string.IsNullOrWhiteSpace(s)) return string.Empty;

//        return s.Trim()
//            .Replace("<STX>", "\x02")   // STX
//            .Replace("<ETX>", "\x03")   // ETX
//            .Replace("<ETB>", "\x17")   // ETB (multi-frame)
//            .Replace("<CR><LF>", "\r")  // ASTM uses CR as record terminator
//            .Replace("<CR>", "\r")
//            .Replace("<LF>", "\n");     // keep \n if you like for readability
//    }

//    public static bool IsFramed(string s) =>
//       !string.IsNullOrEmpty(s) && s[0] == '\x02' && (s.Contains('\x03') || s.Contains('\x17'));

//    public static string WrapFrame(string payload) => "\x02" + payload + "\x03";

//    // Optional: if you want to also send one "joined" block once
//    public static string JoinFrames(IEnumerable<string> frames)
//        => string.Concat(frames.Select(ReplaceTokens));
//}

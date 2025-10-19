// HMS.Communication/Application/Protocols/ASTM/AstmRecordModels.cs
namespace HMS.Communication.Application.Protocols.ASTM
{
    public enum AstmRecType { H, P, O, R, L, Q, C, S, M, Unknown }

    public static class AstmRec
    {
        public static AstmRecType Type(string line)
            => line.Length > 0
                ? line[0] switch
                {
                    'H' => AstmRecType.H,
                    'P' => AstmRecType.P,
                    'O' => AstmRecType.O,
                    'R' => AstmRecType.R,
                    'L' => AstmRecType.L,
                    'Q' => AstmRecType.Q,
                    'C' => AstmRecType.C,
                    'S' => AstmRecType.S,
                    'M' => AstmRecType.M,
                    _ => AstmRecType.Unknown
                }
                : AstmRecType.Unknown;
    }
}

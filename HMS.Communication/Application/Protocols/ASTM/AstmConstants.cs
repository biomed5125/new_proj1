// HMS.Communication/Application/Protocols/ASTM/AstmConstants.cs
namespace HMS.Communication.Application.Protocols.ASTM
{
    public static class AstmConstants
    {
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
    }
}
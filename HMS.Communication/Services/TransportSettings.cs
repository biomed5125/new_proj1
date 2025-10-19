
namespace HMS.Communication.Application.Services
{
    public sealed class TransportSettings
    {
        public string? PortName { get; set; }
        public int Baud { get; set; } = 9600;
        public System.IO.Ports.Parity Parity { get; set; } = System.IO.Ports.Parity.None;
        public int DataBits { get; set; } = 8;
        public System.IO.Ports.StopBits StopBits { get; set; } = System.IO.Ports.StopBits.One;

        public string? Host { get; set; }
        public int Port { get; set; }
    }
}

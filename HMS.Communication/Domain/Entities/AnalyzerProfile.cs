using HMS.Communication.Infrastructure.Persistence.Entities;

namespace HMS.Communication.Domain.Entities
{
    public sealed class AnalyzerProfile
    {
        public long AnalyzerProfileId { get; set; }
        public string Name { get; set; } = default!;          // "Roche e411"
        public string Protocol { get; set; } = default!;      // "ASTM", "SUIT", "ASCII"
        public string DriverClass { get; set; } = default!;   // e.g. "RocheCobasDriver"
        public string PortSettings { get; set; } = "{}";      // JSON: { Baud:9600, Bits:8, Parity:"None", Stop:1 }
        public string? DefaultMode { get; set; }              // "Cobas", "Elecsys"
        public string? Notes { get; set; }
        public string CreatedBy { get; set; }= default!;
        public DateTime CreatedAt { get; set; }

        public ICollection<CommDevice> Devices { get; set; } = new List<CommDevice>();
    }
}

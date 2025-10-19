namespace HMS.Communication.Domain.CommEntities
{
    public sealed class Device
    {
        public long DeviceId { get; set; }                 // PK
        public string Name { get; set; } = default!;
        public string Vendor { get; set; } = "Roche";
        public string Model { get; set; } = default!;      // c311, e411, 6000
        public string DriverKey { get; set; } = default!;  // "RocheCobas:c311"
        public string Transport { get; set; } = "Serial";  // "Serial" | "Tcp" | "File"
        public string TransportSettingsJson { get; set; } = "{}"; // { "PortName":"COM3", "Baud":9600, ... }
        public string ParserOptionsJson { get; set; } = "{}";     // batch sizes, retries
        public bool Enabled { get; set; } = true;
    }
}

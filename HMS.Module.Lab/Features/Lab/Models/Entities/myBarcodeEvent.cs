
namespace HMS.Module.Lab.Features.Lab.Models.Entities
{
    // HMS.Module.Lab.Features.Lab.Models.Entities
    public class myBarcodeEvent
    {
        public long BarcodeEventId { get; set; }
        public string AccessionNumber { get; set; } = "";
        public string Event { get; set; } = ""; // "ISSUED" | "SCANNED"
        public DateTimeOffset At { get; set; }
        public string? Who { get; set; }
        public string? Note { get; set; }
    }
}

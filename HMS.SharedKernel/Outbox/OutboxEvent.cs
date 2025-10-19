using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SharedKernel.Outbox
{
    public class OutboxEvent
    {
        public long OutboxEventId { get; set; }
        public string Stream { get; set; } = "";         // "patient" | "appointment"
        public string Type { get; set; } = "";           // "Patient.Upserted" | "Patient.Deleted" | ...
        public string Payload { get; set; } = "";        // JSON body
        public DateTime OccurredAtUtc { get; set; }
        public DateTime? ProcessedAtUtc { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Communication.Domain.CommEntities
{
    public sealed class InboxBatch
    {
        public long InboxBatchId { get; set; }
        public long DeviceId { get; set; }
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
        public bool Parsed { get; set; } = false;
        public string? ParseError { get; set; }
    }

}

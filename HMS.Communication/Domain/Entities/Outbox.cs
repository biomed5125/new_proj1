namespace HMS.Communication.Domain.CommEntities
{
    public sealed class Outbox
    {
        public long OutboxId { get; set; }
        public long DeviceId { get; set; }
        public string AccessionNumber { get; set; } = default!;
        public string OrderNo { get; set; } = default!;
        public string Payload { get; set; } = default!; // serialized order bundle (we send as ASTM in driver)
        public OutboxState State { get; set; } = OutboxState.Pending;
        public int Retries { get; set; }
        public string? LastError { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

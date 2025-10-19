namespace HMS.Communication.Domain.CommEntities
{
    public sealed class InboxFrame
    {
        public long InboxFrameId { get; set; }
        public long DeviceId { get; set; }
        public long BatchId { get; set; }
        public int FrameNo { get; set; }
        public string RawText { get; set; } = default!; // raw frame text (after STX..ETX), no control bytes
        public string Checksum { get; set; } = default!; // two ASCII hex
        public bool IsNak { get; set; }
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    }

}

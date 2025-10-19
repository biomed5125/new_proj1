

namespace HMS.Module.Lab.Features.Lab.Models.Dtos
{
    public sealed class EnterResultsDto
    {
        public List<Entry> Items { get; set; } = new();
        public sealed class Entry
        {
            public long LabRequestItemId { get; set; }
            public string? Value { get; set; }
            public string? Unit { get; set; }
            public decimal? RefLow { get; set; }
            public decimal? RefHigh { get; set; }
            public string? Flag { get; set; }
        }
    }

}

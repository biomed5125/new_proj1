namespace HMS.Module.Lab.Features.Lab.Models.Entities;
public sealed class myInstrumentMapping
{
    public long InstrumentMappingId { get; set; }
    public string InstrumentCode { get; set; } = default!;
    public string LocalTestCode { get; set; } = default!;
    public string? Notes { get; set; }
    public bool IsDeleted { get; set; }
}

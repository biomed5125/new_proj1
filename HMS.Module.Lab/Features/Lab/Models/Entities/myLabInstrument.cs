namespace HMS.Module.Lab.Features.Lab.Models.Entities;

public sealed class myLabInstrument
{
    public long LabInstrumentId { get; set; }
    public string Name { get; set; } = default!;
    public string? MakeModel { get; set; }
    public string? ConnectionType { get; set; }    // TCP, Serial, FileDrop, HL7, ASTM...
    public string? Host { get; set; }
    public int? Port { get; set; }
    public bool IsActive { get; set; } = true;

    // audit
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}

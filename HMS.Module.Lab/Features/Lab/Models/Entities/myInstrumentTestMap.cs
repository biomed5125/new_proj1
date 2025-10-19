// HMS.Module.Lab/Features/Lab/Models/Entities/myInstrumentTestMap.cs
namespace HMS.Module.Lab.Features.Lab.Models.Entities;

public sealed class myInstrumentTestMap
{
    public long InstrumentTestMapId { get; set; }

    // Which analyzer (from your Communication project). We keep it scalar to avoid cross-DB FK.
    public long DeviceId { get; set; }

    // Which LIS test it maps to
    public long LabTestId { get; set; }
    public string InstrumentTestCode { get; set; } = default!;
    public string LabTestCode { get; set; } = default!;        // e.g., "GLU"

    // audit / soft delete
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}

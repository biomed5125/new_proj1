// Features/Lab/Models/Entities/myLabTest.cs
namespace HMS.Module.Lab.Features.Lab.Models.Entities;

public sealed class myLabTest
{
    public long LabTestId { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? SpecimenType { get; set; }
    public string? ServiceCode { get; set; }
    public int TatMinutes { get; set; }
    public bool IsActive { get; set; } = true;

    // audit
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}

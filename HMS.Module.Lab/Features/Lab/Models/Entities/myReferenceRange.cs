namespace HMS.Module.Lab.Features.Lab.Models.Entities;
public sealed class myReferenceRange
{
    public long ReferenceRangeId { get; set; }
    public decimal? RefLow { get; set; }
    public decimal? RefHigh { get; set; }
    public string? Note { get; set; }
    // 🔹 add this
    public long? LabTestId { get; set; }        // FK (nullable if optional)
    public myLabTest? LabTest { get; set; }     // <- navigation

    // audit
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}

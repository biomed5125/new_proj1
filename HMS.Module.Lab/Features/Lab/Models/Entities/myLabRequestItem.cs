namespace HMS.Module.Lab.Features.Lab.Models.Entities;
public sealed class myLabRequestItem
{
    public long LabRequestItemId { get; set; }

    // FKs
    public long LabRequestId { get; set; }
    public long LabTestId { get; set; }

    // LIS test code (e.g., "GLU", "NA")
    public string LabTestCode { get; set; } = default!;
    public string? LabTestName { get; set; }
    public string? LabTestUnit { get; set; }
    public decimal? LabTestPrice { get; set; }
    // audit
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }

    // Navs renamed to match FK names → EF picks these up automatically
    public myLabRequest LabRequest { get; set; } = default!;
    public myLabTest LabTest { get; set; } = default!;

    // optional but useful if you relate results to the item
    public ICollection<myLabResult> Results { get; set; } = new List<myLabResult>();
}

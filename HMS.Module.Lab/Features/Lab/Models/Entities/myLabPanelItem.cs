namespace HMS.Module.Lab.Features.Lab.Models.Entities;
public sealed class myLabPanelItem
{
    public long LabPanelItemId { get; set; }
    public long LabPanelId { get; set; }
    public long LabTestId { get; set; }
    public int SortOrder { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public myLabPanel Panel { get; set; } = default!;
    public myLabTest Test { get; set; } = default!;
}

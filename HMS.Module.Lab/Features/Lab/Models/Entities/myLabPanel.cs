namespace HMS.Module.Lab.Features.Lab.Models.Entities;
public sealed class myLabPanel
{
    public long LabPanelId { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool IsActive { get; set; } = true;

    // audit
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<myLabPanelItem> Items { get; set; } = new List<myLabPanelItem>();
}

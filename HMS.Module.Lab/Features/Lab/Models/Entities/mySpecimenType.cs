namespace HMS.Module.Lab.Features.Lab.Models.Entities;
public sealed class mySpecimenType
{
    public long SpecimenTypeId { get; set; }
    public string Name { get; set; } = default!;
    public string? Code { get; set; }

    // audit
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}

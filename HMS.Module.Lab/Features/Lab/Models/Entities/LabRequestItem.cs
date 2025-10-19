// Features/Lab/Models/Entities/myLabRequestItem.cs
namespace HMS.Module.Lab.Features.Lab.Models.Entities;

public sealed class myLabRequestItem
{
    public long LabRequestItemId { get; set; }
    public long LabRequestId { get; set; }
    public long LabTestId { get; set; }

    // audit
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public bool IsDeleted { get; set; }
}

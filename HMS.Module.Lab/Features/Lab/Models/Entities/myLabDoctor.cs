namespace HMS.Module.Lab.Features.Lab.Models.Entities;

public sealed class myLabDoctor
{
    public long LabDoctorId { get; set; }
    public string FullName { get; set; } = "";
    public string? LicenseNo { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Specialty { get; set; }

    // audit
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public ICollection<myLabRequest> Requests { get; set; } = new List<myLabRequest>();
}

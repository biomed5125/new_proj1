namespace HMS.Module.Lab.Features.Lab.Models.Entities;

public sealed class myLabPatient
{
    public long LabPatientId { get; set; }

    // local patient fields (keep optional to stay flexible)
    public string? Mrn { get; set; }             // local MRN (if you want)
    public string FullName { get; set; } = "";
    public DateTime? DateOfBirth { get; set; }
    public string? Sex { get; set; }              // "M"/"F"/"U" (or enum later)
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }

    // audit
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public ICollection<myLabRequest> Requests { get; set; } = new List<myLabRequest>();
    public myLabPatientProfile? Profile { get; set; }
}

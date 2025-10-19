namespace HMS.Core.ReadModels;

public class CoreDoctor
{
    public long DoctorId { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string FullName { get; set; } = "";
    public string LicenseNumber { get; set; } = "";
    public string Specialty { get; set; } = "";
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public DateTime? HireDateUtc { get; set; }
}

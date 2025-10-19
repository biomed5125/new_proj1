// Features/Doctor/Models/Entities/myDoctor.cs
namespace HMS.Module.Doctor.Features.Doctor.Models.Entities;

public sealed class myDoctor
{
    public long DoctorId { get; set; }

    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string FullName => $"{FirstName} {LastName}";
    public string LicenseNumber { get; set; } = default!;
    public string? Specialty { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? HireDateUtc { get; set; }

    // audit / soft-delete
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}

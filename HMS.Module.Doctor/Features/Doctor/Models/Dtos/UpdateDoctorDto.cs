// Features/Doctor/Models/Dtos/UpdateDoctorDto.cs
namespace HMS.Module.Doctor.Features.Doctor.Models.Dtos;

public sealed class UpdateDoctorDto
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Specialty { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? HireDateUtc { get; set; }
}

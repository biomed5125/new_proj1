// Features/Doctor/Models/Dtos/DoctorDto.cs
namespace HMS.Module.Doctor.Features.Doctor.Models.Dtos;

public sealed record DoctorDto(
    long DoctorId,
    string FirstName,
    string LastName,
    string FullName,
    string LicenseNumber,
    string? Specialty,
    string? Phone,
    string? Email,
    bool IsActive,
    DateTime? HireDateUtc
);

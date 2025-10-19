// Features/Doctor/Queries/DoctorQuery.cs
namespace HMS.Module.Doctor.Features.Doctor.Queries;

public sealed class DoctorQuery
{
    public string? Search { get; set; }     // matches name, license, phone, email
    public bool? IsActive { get; set; }    // filter active/inactive
}

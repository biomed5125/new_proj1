// src/HMS.Api/Features/Patient/Models/Entities/Patient.cs
using HMS.SharedKernel.Base;
namespace HMS.Module.Patient.Features.Patient.Models.Entities;

public class myPatient : BaseEntity<long>
{
    public long PatientId
    {
        get => Id;          // keep this wrapper so AutoMapper and JSON use PatientId
        set => Id = value;
    }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    // Patient
    public string? Mrn { get; set; } = string.Empty;                     // unique, printed, barcoded
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public new bool IsDeleted { get; set; }=false;

    public new string? CreatedBy { get; set; }
    public DateTime? CreateAt { get; set; }
    public new DateTime? UpdatedAt { get; set; }
    public new string? UpdatedBy { get; set; }
}

namespace HMS.Module.Patient.Features.Patient.Models.Dtos
{
    public sealed class PatientDto
    {
        public long PatientId { get; init; }
        public string Mrn { get; init; } = "";
        public string FirstName { get; init; } = "";
        public string LastName { get; init; } = "";
        public DateTime? DateOfBirth { get; init; }   // <- include DoB
        public string? Gender { get; init; }          // keep type aligned with your entity
        public string? Phone { get; init; }
        public string? Email { get; init; }
        public DateTime CreatedAt { get; set; }
        public string FullName => $"{FirstName} {LastName}";

    }
}

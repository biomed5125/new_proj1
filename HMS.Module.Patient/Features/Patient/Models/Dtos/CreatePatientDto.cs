namespace HMS.Module.Patient.Features.Patient.Models.Dtos
{
    public class CreatePatientDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        public string Mrn { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}

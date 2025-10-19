namespace HMS.Module.Lab.Features.Lab.Models.Dtos;

public sealed record LabPatientDto(long LabPatientId, string FullName, string? Mrn, DateTime? DateOfBirth, string? Sex, string? Phone, string? Email);
public sealed record UpsertPatientDto(string FullName, string? Mrn, DateTime? DateOfBirth, string? Sex, string? Phone, string? Email, string? Address);

public sealed record LabDoctorDto(long LabDoctorId, string FullName, string? LicenseNo, string? Phone, string? Email);
public sealed record UpsertDoctorDto(string FullName, string? LicenseNo, string? Phone, string? Email);

// manual order (LIS mode)
public sealed record ManualOrderCreateDto(
    long LabPatientId,
    long? LabDoctorId,
    IReadOnlyList<long> TestIds     // choose tests by id
);

public sealed record ManualOrderCreateResponse(
    long LabRequestId,
    string OrderNo,
    long LabSampleId,
    string AccessionNumber
);

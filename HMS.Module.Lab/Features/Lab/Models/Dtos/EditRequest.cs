namespace HMS.Module.Lab.Features.Lab.Models.Dtos
{

    public sealed record EditHeadDto(
        long LabRequestId,
        string OrderNo,
        string? PatientDisplay,
        long? PatientId,
        long? LabPatientId,
        string? DoctorDisplay,
        long? DoctorId,
        long? LabDoctorId,
        string? Source,
        DateTime CreatedAtUtc,
        bool LockReasonHasAnyResult);

    public sealed record EditDataDto(
        EditHeadDto Head,
        IReadOnlyList<TestRow> Items);
    public sealed record TestRow(long LabTestId, string Code, string Name, string? Unit, decimal? Price, bool Selected);

    public sealed record ApplyDto(
        long LabRequestId,
        long? DoctorId,        // cross-module
        long? LabDoctorId,     // LIS local
        string? DoctorName,    // if you want to store display-only
        IReadOnlyList<long> TestIds);
}
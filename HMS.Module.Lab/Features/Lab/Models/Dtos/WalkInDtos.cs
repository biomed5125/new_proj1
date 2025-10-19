// HMS.Module.Lab/Features/Lab/Models/Dtos/WalkInDtos.cs

namespace HMS.Module.Lab.Features.Lab.Models.Dtos
{

    public sealed record CreateWalkInOrderDto(
        long? PatientId,          // prefer ID
        string? PatientName,      // fallback: free text
        string? Sex,              // optional when free text
        DateTime? Dob,            // optional when free text
        long? DoctorId,           // prefer ID
        string? DoctorName,       // fallback: free text
        IReadOnlyList<long> TestIds,
        bool CollectNow
    );

    public sealed record WalkInCreatedDto(
        long LabRequestId,
        string OrderNo,
        long LabSampleId,
        string AccessionNumber
    );

}

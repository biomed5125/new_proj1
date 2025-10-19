namespace HMS.Module.Lab.Features.Lab.Models.Dtos;

public sealed record CreateLabRequestDto(
    long PatientId,
    long? AdmissionId,
    long? DoctorId,
    string Priority,
    string? Notes,
    IReadOnlyList<long>? TestIds,
    IReadOnlyList<long>? PanelIds
);

public sealed record CollectSampleDto(long LabRequestId, string Collector);
public sealed record ReceiveSampleDto(long LabSampleId, string Receiver);

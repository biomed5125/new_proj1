namespace HMS.Module.Admission.Features.Admission.Queries;

public sealed class AdmissionQuery
{
    public long? PatientId { get; init; }
    public int? Status { get; init; }             // map from enum if provided
    public DateTime? FromUtc { get; init; }
    public DateTime? ToUtc { get; init; }
}

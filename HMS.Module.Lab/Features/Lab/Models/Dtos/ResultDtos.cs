namespace HMS.Module.Lab.Features.Lab.Models.Dtos;

public sealed record EnterResultDto(long LabRequestItemId, string? Value, string? Unit);
public sealed record ResultLineDto(long LabRequestItemId, string TestCode, string TestName, string? Value, string? Unit, decimal? RefLow, decimal? RefHigh, string Flag);
public sealed record ResultDetailDto(long LabRequestId, string OrderNo, IEnumerable<ResultLineDto> Lines);
public sealed class ApproveResultsDto { public long LabRequestId { get; set; } public long ApprovedByDoctorId { get; set; } }

// Features/Lab/Services/ILabService.cs
using HMS.Module.Lab.Features.Lab.Models.Dtos;
using HMS.Module.Lab.Features.Lab.Models.Enums;

namespace HMS.Module.Lab.Features.Lab.Services;

public interface ILabService
{
    Task<List<ResultDtos>> ListTestsAsync(CancellationToken ct);
    Task<LabRequestDto> CreateRequestAsync(CreateLabRequestDto dto, string? user, CancellationToken ct);
    Task<LabRequestDto?> GetAsync(long id, CancellationToken ct);
    Task<List<LabRequestDto>> ListAsync(DateTime? fromUtc, DateTime? toUtc, long? patientId, LabRequestStatus? status, CancellationToken ct);
    Task<bool> AddItemsAsync(long requestId, AddItemsDto dto, string? user, CancellationToken ct);
    Task<bool> CollectSampleAsync(long requestId, DateTime whenUtc, string? by, CancellationToken ct);
    Task<bool> ReceiveSampleAsync(long requestId, DateTime whenUtc, string? by, CancellationToken ct);
    Task<bool> EnterResultsAsync(long requestId, EnterResultsDto dto, string? user, CancellationToken ct);
    Task<bool> ApproveAsync(long requestId, ApproveDto dto, string? user, CancellationToken ct);
}

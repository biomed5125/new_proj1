using HMS.Module.Lab.Features.Lab.Models.Dtos;

namespace HMS.Module.Lab.Service;
public interface ILabResultService
{
    Task EnterAsync(IEnumerable<EnterResultDto> lines, CancellationToken ct);
    Task<bool> ApproveAsync(long labRequestId, long approvedByDoctorId, CancellationToken ct);
}

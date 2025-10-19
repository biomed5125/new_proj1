using HMS.Module.Lab.Features.Lab.Models.Dtos;

namespace HMS.Module.Lab.Service;
public interface ILabOrderService
{
    Task<long> CreateRequestAsync(CreateLabRequestDto dto, CancellationToken ct);
}

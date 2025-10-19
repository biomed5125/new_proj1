using HMS.Module.Lab.Features.Lab.Models.Entities;

namespace HMS.Module.Lab.Repositories;
public interface ILabWriteRepo
{
    Task AddRequest(myLabRequest req, CancellationToken ct);
    Task AddSample(myLabSample sample, CancellationToken ct);
    Task Save(CancellationToken ct);
}

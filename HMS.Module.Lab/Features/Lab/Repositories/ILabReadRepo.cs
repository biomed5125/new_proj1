using HMS.Module.Lab.Features.Lab.Models.Dtos;
using HMS.Module.Lab.Features.Lab.Models.Entities;

namespace HMS.Module.Lab.Repositories;
public interface ILabReadRepo
{
    Task<myLabRequest?> GetRequest(long id, CancellationToken ct);
    IQueryable<myLabTest> QueryTests();
    IQueryable<myLabPanel> QueryPanels();
}

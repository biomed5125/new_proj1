using HMS.Module.Lab.Features.Lab.Models.Dtos;
using HMS.Module.Lab.Features.Lab.Models.Entities;

namespace HMS.Module.Lab.Service;
public interface ILabCatalogService
{
    Task<myLabTest> UpsertTestAsync(UpsertTestDto dto, CancellationToken ct);
    Task<myLabPanel> UpsertPanelAsync(UpsertPanelDto dto, CancellationToken ct);

    // NEW:
    Task<myLabTest?> GetTestAsync(long id, CancellationToken ct);
    Task<bool> DeleteTestAsync(long id, CancellationToken ct);   // soft if referenced
    Task<bool> ToggleTestAsync(long id, bool active, CancellationToken ct);

    Task<myLabPanel?> GetPanelAsync(long id, CancellationToken ct);
    Task<bool> DeletePanelAsync(long id, CancellationToken ct);
}

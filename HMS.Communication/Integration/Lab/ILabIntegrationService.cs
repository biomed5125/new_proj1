using HMS.Module.Lab.Features.Lab.Models.Enums;

namespace HMS.Communication.Integration.Lab;

public interface ILabIntegrationService
{
    Task UpsertResultAsync(
        string accession,
        long deviceId,
        string instrumentTestCode,
        string? mappedLisCode,
        string? value,
        string? units,
        string? flag,
        string? notes,
        CancellationToken ct);
}

// HMS.Communication/Integration/Lab/LabIntegrationService.cs
using HMS.Communication.Integration.Lab;
using HMS.Module.Lab.Features.Lab.Service;
using HMS.Module.Lab.Features.Lab.Models.Enums;
using HMS.Realtime.Abstractions;

namespace HMS.Communication.Integration;

public sealed class LabIntegrationService : ILabIntegrationService
{
    private readonly ILabResultWriter _writer;
    private readonly ILabRealtime _rt;

    public LabIntegrationService(ILabResultWriter writer, ILabRealtime rt)
    {
        _writer = writer;
        _rt = rt; // Null in Host, SignalR in API
    }

    public async Task UpsertResultAsync(
        string accession,
        long deviceId,
        string instrumentTestCode,
        string? mappedLisCode,
        string? value,
        string? units,
        string? flag,
        string? notes,
        CancellationToken ct)
    {
        ResultFlag? mappedFlag = flag?.ToUpperInvariant() switch
        {
            "H" or "HIGH" => ResultFlag.High,
            "L" or "LOW" => ResultFlag.Low,
            "C" or "CRIT" => ResultFlag.CriticalHigh,
            _ => null
        };

        await _writer.UpsertResultAsync(
            accession: accession,
            deviceId: deviceId,
            instrumentTestCode: instrumentTestCode,
            value: value,
            units: units,
            flag: mappedFlag,
            rawFlagOrNotes: notes ?? flag,
            ct: ct
        );

        // fire-and-forget UI notify (safe if NullLabRealtime)
        _ = _rt.BroadcastAsync(new(
            At: DateTimeOffset.UtcNow,
            DeviceId: deviceId,
            Kind: "ResultPosted",
            Accession: accession,
            InstrumentCode: instrumentTestCode,
            Value: value,
            Units: units,
            Flag: flag
        ), ct);
    }
}

using HMS.Communication.Integration.Lab;
using HMS.Communication.Application.Mapping;
using HMS.Communication.Abstractions;

namespace HMS.Communication.Services;

public sealed class ResultIngestService
{
    private readonly RocheCobasAstmMapper _mapper;
    private readonly ILabIntegrationService _lab;

    public ResultIngestService(RocheCobasAstmMapper mapper, ILabIntegrationService lab)
    { _mapper = mapper; _lab = lab; }

    public async Task HandleAsync(InboundEnvelope env, CancellationToken ct)
    {
        if (env.ParsedRecordType?.Equals("R", StringComparison.OrdinalIgnoreCase) != true) return;
        var dto = _mapper.MapResult(env); // parse value, units, flags from frame raw (or env fields)
        if (dto is null) return;

        await _lab.UpsertResultAsync(
        accessionNumber: dto.AccessionNumber!,
        instrumentTestCode: dto.InstrumentTestCode!,
        deviceId: env.DeviceId,                         // <— pass numeric id
        value: dto.Value!,
        units: dto.UnitNormalized,                      // <— normalized
        abnormalFlag: dto.FlagNameNormalized,           // <— normalized
        observedAt: DateTimeOffset.UtcNow,
        ct: ct);
    }
}

// HMS.Communication/Routing/DefaultMessageRouter.cs
using HMS.Communication.Abstractions;
using HMS.Communication.Application.Mapping;
using HMS.Communication.Integration.Lab;
using Microsoft.Extensions.Logging;

namespace HMS.Communication.Routing
{
    public sealed class DefaultMessageRouter : IMessageRouter
    {
        private readonly IInstrumentToLisMapper _mapper;
        private readonly ILabIntegrationService _lab;
        private readonly ILogger<DefaultMessageRouter> _log;

        public DefaultMessageRouter(
            IInstrumentToLisMapper mapper,
            ILabIntegrationService lab,
            ILogger<DefaultMessageRouter> log)
        {
            _mapper = mapper;
            _lab = lab;
            _log = log;
        }

        public async Task RouteAsync(NormalizedEvent ev, CancellationToken ct)
        {
            if (ev.Kind != EventKind.ResultPosted) return;
            if (string.IsNullOrWhiteSpace(ev.Accession)) return;

            try
            {
                var lisCode = ev.InstrumentCode is null ? null : _mapper.Map(ev.Device.Id, ev.InstrumentCode);

                await _lab.UpsertResultAsync(
                    accession: ev.Accession!,
                    deviceId: ev.Device.Id,
                    instrumentTestCode: ev.InstrumentCode ?? "",
                    mappedLisCode: lisCode,
                    value: ev.Value,
                    units: ev.Units,
                    flag: ev.Flag,
                    notes: ev.Notes,
                    ct: ct
                );
            }
            catch (Exception ex)
            {
                _log.LogError(ex,
                    "Router failed to upsert result. Accession={Accession}, DeviceId={DeviceId}, InstrCode={Instr}, Val={Val} {Units}",
                    ev.Accession, ev.Device.Id, ev.InstrumentCode, ev.Value, ev.Units);
                // swallow to keep the worker running
            }
        }
    }
}

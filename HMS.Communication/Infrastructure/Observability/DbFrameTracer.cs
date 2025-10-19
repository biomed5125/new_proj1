using HMS.Communication.Abstractions;
using HMS.Communication.Domain.Entities;
using HMS.Communication.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;

public sealed class DbFrameTracer : IFrameTracer
{
    private readonly IDbContextFactory<CommunicationDbContext> _factory;
    private readonly ILogger<DbFrameTracer> _log;
    public DbFrameTracer(IDbContextFactory<CommunicationDbContext> factory, ILogger<DbFrameTracer> log)
    { _factory = factory; _log = log; }

    public async Task TraceAsync(RawFrame frame, CancellationToken ct)
    {
        try
        {
            await using var db = await _factory.CreateDbContextAsync(ct);

            var atUtc = frame.At.UtcDateTime;
            var ascii = frame.Ascii ?? "";
            var bytes = frame.Bytes ?? Array.Empty<byte>();
            var devId = frame.Device.Id;
            var payload = ascii.Length > 0 ? ascii : Encoding.ASCII.GetString(bytes);

            if (frame.Dir == FrameDirection.Rx)
            {
                db.Inbound.Add(new CommMessageInbound
                {
                    DeviceId = devId,
                    At = atUtc,
                    Direction = "RX",
                    Transport = frame.Transport,
                    Ascii = ascii,
                    Bytes = bytes,
                    BusinessNo = ""
                });
            }
            else
            {
                db.Outbound.Add(new CommMessageOutbound
                {
                    DeviceId = devId,
                    At = atUtc,
                    Transport = frame.Transport,
                    Payload = payload,
                    Sent = true,
                    BusinessNo = ""
                });
            }

            await db.SaveChangesAsync(ct);
            _log.LogInformation("DbFrameTracer wrote {dir} for device {dev} at {at}",
                frame.Dir, frame.Device.Code, frame.At);
        }
        catch (OperationCanceledException) { /* shut down */ }
        catch (Exception ex)
        {
            _log.LogError(ex, "DbFrameTracer failed.");
            throw; // ← TEMP: let’s see the root cause in Host console
        }
    }
}

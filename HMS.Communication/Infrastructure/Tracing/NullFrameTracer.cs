using System.Threading;
using System.Threading.Tasks;
using HMS.Communication.Domain.Abstractions;

namespace HMS.Communication.Infrastructure.Tracing
{
    /// <summary>No-op tracer so the driver/host can depend on IFrameTracer safely.</summary>
    public sealed class NullFrameTracer : IFrameTracer
    {
        public Task InAsync(long deviceId, string deviceCode, string raw, string note, CancellationToken ct)
            => Task.CompletedTask;

        public Task OutAsync(long deviceId, string deviceCode, string raw, string note, CancellationToken ct)
            => Task.CompletedTask;
    }
}

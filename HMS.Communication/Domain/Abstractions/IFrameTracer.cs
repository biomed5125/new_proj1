
namespace HMS.Communication.Domain.Abstractions
{
    // HMS.Communication.Domain.Abstractions
    public interface IFrameTracer
    {
        Task InAsync(long deviceId, string deviceCode, string raw, string note, CancellationToken ct);
        Task OutAsync(long deviceId, string deviceCode, string raw, string note, CancellationToken ct);
    }

}

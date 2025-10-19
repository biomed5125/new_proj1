using System.Buffers;

namespace HMS.Communication.Domain.Abstractions;

public interface ITransport : IAsyncDisposable
{
    event EventHandler<byte[]>? FrameReceivedRaw;  // deliver whole bytes [STX..LF]
    Task StartAsync(CancellationToken ct);
}
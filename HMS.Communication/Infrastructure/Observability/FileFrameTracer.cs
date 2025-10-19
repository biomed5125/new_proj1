// HMS.Communication.Infrastructure/Observability/FileFrameTracer.cs
using System.Text;
using HMS.Communication.Abstractions;

namespace HMS.Communication.Infrastructure.Observability;

public sealed class FileFrameTracer : IFrameTracer, IDisposable
{
    private readonly string _filePath;
    private readonly object _sync = new();

    public FileFrameTracer(string filePath)
    {
        _filePath = filePath;
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
    }

    public Task TraceAsync(RawFrame frame, CancellationToken ct)
    {
        try
        {
            var line = $"[{frame.At:yyyy-MM-dd HH:mm:ss.fff}] {frame.Dir} {frame.Device.Code} " +
                       $"{frame.Transport}: {frame.Ascii?.Replace('\r', '|').Replace('\n', ' ')}";

            lock (_sync)
            {
                File.AppendAllText(_filePath, line + Environment.NewLine, Encoding.UTF8);
            }
        }
        catch
        {
            // Ignore file IO issues so tracing never breaks communication.
        }

        return Task.CompletedTask;
    }

    public void Dispose() { /* no-op */ }
}

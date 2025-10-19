using System.Collections.Concurrent;
using System.IO;
using System.Text;

namespace HMS.CommBench.Core;

public sealed class TraceLog : IDisposable
{
    private readonly ConcurrentQueue<string> _ring = new();
    private readonly int _max = 2000;
    private readonly string? _file;
    private readonly object _fileLock = new();

    public TraceLog(string? filePath = null)
    {
        _file = filePath;
        if (!string.IsNullOrWhiteSpace(_file))
        {
            var dir = Path.GetDirectoryName(_file)!;
            if (!string.IsNullOrWhiteSpace(dir)) Directory.CreateDirectory(dir);
        }
    }

    public void Add(string dir, string text)
    {
        var line = $"{DateTime.Now:HH:mm:ss.fff}\t{dir}\t{text}";
        _ring.Enqueue(line);
        while (_ring.Count > _max && _ring.TryDequeue(out _)) { }
        if (!string.IsNullOrWhiteSpace(_file))
        {
            lock (_fileLock)
            {
                File.AppendAllText(_file!, line + Environment.NewLine, Encoding.UTF8);
            }
        }
    }

    public string[] Snapshot() => _ring.ToArray();

    public void Dispose() { }
}

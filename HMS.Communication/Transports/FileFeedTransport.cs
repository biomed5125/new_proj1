// HMS.Communication/Transports/FileFeedTransport.cs
using HMS.Communication.Abstractions;

namespace HMS.Communication.Transports
{
    public sealed class FileFeedTransport : ITransport
    {
        private readonly string _path;
        private FileStream? _fs;
        private bool _eof;

        public string Name => "FileFeed";
        public FileFeedTransport(string path) => _path = path;

        public Task OpenAsync(CancellationToken ct)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            // open for read, start at BEGINNING
            _fs = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _fs.Position = 0;
            _eof = false;
            return Task.CompletedTask;
        }

        public Task CloseAsync(CancellationToken ct)
        {
            _fs?.Dispose();
            _fs = null;
            return Task.CompletedTask;
        }

        public async Task<int> ReadAsync(Memory<byte> buffer, CancellationToken ct)
        {
            if (_fs is null || _eof) return 0;

            // read remaining bytes once; then signal EOF
            var remaining = (int)Math.Min(buffer.Length, _fs.Length - _fs.Position);
            if (remaining <= 0)
            {
                _eof = true;
                return 0;
            }

            return await _fs.ReadAsync(buffer[..remaining], ct);
        }

        // not used for file feed; keep as no-op
        public Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken ct)
            => Task.CompletedTask;

        public ValueTask DisposeAsync()
        {
            _fs?.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}

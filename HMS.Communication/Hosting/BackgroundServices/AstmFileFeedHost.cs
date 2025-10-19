// HMS.Communication.Hosting/AstmFileFeedHost.cs
using HMS.Communication.Abstractions;
using HMS.Communication.Sessions;
using HMS.Communication.Transports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HMS.Communication.Hosting;
public sealed class AstmFileFeedHostOptions
{
    public string InputPath { get; set; } = @"D:\HMS\comm\in";
    public string Pattern { get; set; } = "*.txt";
    public string? ArchiveFolder { get; set; } = @"D:\HMS\comm\archive";
    public int PollMs { get; set; } = 750;

    // which device profile to attribute frames to
    public long DeviceId { get; set; } = 1;
    public string DeviceCode { get; set; } = "ROCHE1";
}

public sealed class AstmFileFeedHost : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly AstmFileFeedHostOptions _opt;
    private readonly ILogger<AstmFileFeedHost> _log;

    public AstmFileFeedHost(IServiceProvider sp, IOptions<AstmFileFeedHostOptions> opt, ILogger<AstmFileFeedHost> log)
    {
        _sp = sp;
        _opt = opt.Value;
        _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Directory.CreateDirectory(_opt.InputPath);
        if (!string.IsNullOrWhiteSpace(_opt.ArchiveFolder))
            Directory.CreateDirectory(_opt.ArchiveFolder);

        _log.LogInformation("ASTM FileFeed host watching {in} | Pattern: {p} | Archive: {arc}",
            _opt.InputPath, _opt.Pattern, _opt.ArchiveFolder);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                foreach (var file in Directory.EnumerateFiles(_opt.InputPath, _opt.Pattern))
                {
                    // avoid grabbing a file that is still being written
                    var age = DateTime.UtcNow - File.GetLastWriteTimeUtc(file);
                    if (age < TimeSpan.FromMilliseconds(Math.Max(250, _opt.PollMs / 2)))
                        continue;

                    // try to claim it
                    var processing = file + ".processing";
                    try
                    {
                        if (File.Exists(processing)) continue; // someone already claimed it
                        File.Move(file, processing);           // atomic rename
                        _log.LogInformation("Claimed file: {file}", processing);
                    }
                    catch (IOException) { continue; }
                    catch (UnauthorizedAccessException) { continue; }

                    string? finalDest = null;

                    try
                    {
                        using var scope = _sp.CreateScope();

                        // resolve per-file so scoped services (DbContexts, etc.) are safe
                        var adapter = scope.ServiceProvider.GetRequiredService<IProtocolAdapter>();
                        var normalizer = scope.ServiceProvider.GetRequiredService<IRecordNormalizer>();
                        var msgRouter = scope.ServiceProvider.GetRequiredService<IMessageRouter>();
                        var sink = scope.ServiceProvider.GetRequiredService<IEventSink>();
                        var tracer = scope.ServiceProvider.GetRequiredService<IFrameTracer>();

                        _log.LogInformation(
                            "Pipeline resolved ␦ Adapter:{Adapter} | Normalizer:{Norm} | Router:{Router} | Sink:{Sink} | Tracer:{Tracer}",
                            adapter.GetType().Name, normalizer.GetType().Name, msgRouter.GetType().Name,
                            sink.GetType().Name, tracer.GetType().Name);

                        await using var transport = new FileFeedTransport(processing);
                        await transport.OpenAsync(stoppingToken);

                        var channel = new AstmProtocolSession(
                            new DeviceRef(_opt.DeviceId, _opt.DeviceCode),
                            transport);

                        _log.LogInformation("Processing ASTM file {file}", processing);

                        await adapter.RunAsync(channel, normalizer, msgRouter, sink, tracer, stoppingToken);

                        // success path → archive or delete
                        if (!string.IsNullOrWhiteSpace(_opt.ArchiveFolder))
                        {
                            finalDest = Path.Combine(_opt.ArchiveFolder!, 
                                Path.GetFileName(processing).Replace(".processing", ""));
                            _log.LogInformation("Success path → will archive to {dest}", finalDest);
                            _log.LogInformation("Archived {file} → {dest}", processing, finalDest);
                        }
                    }
                    catch (OperationCanceledException) { throw; }
                    catch (Exception ex)
                    {
                        _log.LogError(ex, "Error while processing {file}", processing);

                        // convert to .err for inspection
                        try
                        {
                            var errName = Path.GetFileName(processing).Replace(".processing", ".err");
                            finalDest = string.IsNullOrWhiteSpace(_opt.ArchiveFolder)
                                ? Path.Combine(_opt.InputPath, errName)
                                : Path.Combine(_opt.ArchiveFolder!, errName);
                        }
                        catch { /* ignore */ }
                    }
                    finally
                    {
                        try
                        {
                            if (File.Exists(processing))
                            {
                                if (string.IsNullOrWhiteSpace(finalDest))
                                {
                                    // No archive configured → delete
                                    File.Delete(processing);
                                    _log.LogInformation("Finalized (deleted) {file} (no archive configured).", processing);
                                }
                                else
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(finalDest)!);
                                    File.Move(processing, finalDest, overwrite: true);
                                    _log.LogInformation("Finalized {file} → {dest}", processing, finalDest);
                                }
                            }
                            else
                            {
                                _log.LogInformation("Nothing to finalize; {file} not found.", processing);
                            }
                        }
                        catch (Exception moveEx)
                        {
                            _log.LogError(moveEx, "FINALIZE FAILED for {file}. Will retry next loop.", processing);
                            // Leave as .processing so the next loop can retry
                        }
                    }

                }
            }
            catch (OperationCanceledException) { break; }   // graceful shutdown
            catch (Exception ex)
            {
                _log.LogError(ex, "ASTM FileFeed error; retrying in 1s");
                try { await Task.Delay(1000, stoppingToken); } catch { /* ignore */ }
            }

            try { await Task.Delay(_opt.PollMs, stoppingToken); } catch { /* ignore */ }
        }

        _log.LogInformation("ASTM FileFeed host stopped.");
    }

    private static async Task MoveWithRetryAsync(string src, string dest, int attempts, int delayMs)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
        for (var i = 0; i < attempts; i++)
        {
            try { File.Move(src, dest, overwrite: true); return; }
            catch { await Task.Delay(delayMs); }
        }
        File.Move(src, dest, overwrite: true);
    }

    private static async Task DeleteWithRetryAsync(string path, int attempts, int delayMs)
    {
        for (var i = 0; i < attempts; i++)
        {
            try { File.Delete(path); return; }
            catch { await Task.Delay(delayMs); }
        }
        File.Delete(path);
    }
}

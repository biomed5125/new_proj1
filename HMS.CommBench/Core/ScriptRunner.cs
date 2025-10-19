using System.IO;
using System.Text.Json;

namespace HMS.CommBench.Core;

public sealed class ScriptRunner
{
    private readonly AstmSession _session;

    public ScriptRunner(AstmSession session) => _session = session;

    private sealed record ScriptModel(
        bool startWithEnq,
        bool endWithEot,
        List<string> frames,
        List<int>? delaysMs
    );

    public async Task RunFileAsync(string path, CancellationToken ct)
    {
        if (!File.Exists(path)) throw new FileNotFoundException("Script not found", path);
        var json = await File.ReadAllTextAsync(path, ct);
        var model = JsonSerializer.Deserialize<ScriptModel>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("Invalid script JSON.");

        await RunAsync(model, ct);
    }

    public async Task RunInlineAsync(IEnumerable<string> frames, bool startWithEnq = true, bool endWithEot = true, int delayMs = 100, CancellationToken ct = default)
    {
        var m = new ScriptModel(startWithEnq, endWithEot, frames.ToList(), Enumerable.Repeat(delayMs, frames.Count()).ToList());
        await RunAsync(m, ct);
    }

    private async Task RunAsync(ScriptModel m, CancellationToken ct)
    {
        // We use the simple sequence helper in AstmSession
        // (It already sends ENQ ... frames ... EOT)
        if (!m.startWithEnq && !m.endWithEot)
        {
            // fall back: just send frames “as sequence” (the helper still adds ENQ/EOT);
            // for strict control later we’ll expose SendCtrl/SendFrame on AstmSession.
        }

        await _session.SendSequenceAsync(m.frames, ct);

        // Note: delay hints are not strictly applied here because SendSequenceAsync
        // already contains small waits. If you want exact per-frame delays, you can
        // re-send frames one-by-one here with Task.Delay between them when we expose
        // SendFrameAsync in AstmSession.
    }
}
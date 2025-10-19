using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace HMS.CommBench.Service;

public sealed class CommApiClient
{
    // ---- Bench: send-simple ----
    public sealed record SendSimpleRequest(
        [property: JsonPropertyName("deviceId")] long DeviceId,
        [property: JsonPropertyName("accession")] string Accession,
        [property: JsonPropertyName("testCode")] string TestCode,
        [property: JsonPropertyName("value")] string Value,
        [property: JsonPropertyName("unit")] string Unit
    );

    public sealed record SendSimpleResponse(
        [property: JsonPropertyName("file")] string File,
        [property: JsonPropertyName("size")] int Size
    );

    public async Task<(string file, int size)> SendSimpleAsync(
        long deviceId, string accession, string testCode, string value, string unit, CancellationToken ct = default)
    {
        var req = new SendSimpleRequest(deviceId, accession, testCode, value, unit);
        var res = await _http.PostAsJsonAsync("/api/comm/bench/send-simple", req, ct);
        res.EnsureSuccessStatusCode();
        var body = await res.Content.ReadFromJsonAsync<SendSimpleResponse>(cancellationToken: ct);
        return (body!.File, body.Size);
    }

    // ---- ctor / http ----
    private readonly HttpClient _http;
    public CommApiClient(HttpClient http) => _http = http;

    // ---- DTOs (lists) ----
    public sealed record Row(long Id, DateTimeOffset AtUtc, string DeviceCode, string Text);

    // using System.Text.Json.Serialization;
    public sealed record DeviceRow(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("code")] string Code,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("port")] string? Port,
        [property: JsonPropertyName("profile")] string? Profile
    );

    // ---- Devices ----
    public Task<List<DeviceRow>> GetDevicesAsync(CancellationToken ct = default) =>
        _http.GetFromJsonAsync<List<DeviceRow>>("/api/comm/bench/devices", ct)!;

    // ---- Push single/many frames ----
    public sealed record PushRequest([property: JsonPropertyName("deviceCode")] string DeviceCode,
                                     [property: JsonPropertyName("frames")] List<string> Frames);

    public Task<HttpResponseMessage> PushAsync(string deviceCode, IEnumerable<string> frames, CancellationToken ct) =>
        _http.PostAsJsonAsync("/api/comm/bench/push", new PushRequest(deviceCode, frames.ToList()), ct);

    // ---- Bench feeds ----
    public Task<List<Row>> GetInboundAsync(long? afterId = null, int take = 200, CancellationToken ct = default) =>
        _http.GetFromJsonAsync<List<Row>>($"/api/comm/bench/inbound?afterId={(afterId ?? 0)}&take={take}", ct)!;

    public Task<List<Row>> GetOutboundAsync(long? afterId = null, int take = 200, CancellationToken ct = default) =>
        _http.GetFromJsonAsync<List<Row>>($"/api/comm/bench/outbound?afterId={(afterId ?? 0)}&take={take}", ct)!;

    // ---- Barcodes (helpers) ----
    public sealed record IssueRequest(long PatientId, string[] TestCodes);
    public sealed record IssueResponse(long LabRequestId, long LabSampleId, string AccessionNumber);

    public async Task<IssueResponse> IssueLabelAsync(long patientId, string[] testCodes)
    {
        var res = await _http.PostAsJsonAsync("/api/barcodes/issue", new IssueRequest(patientId, testCodes));
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<IssueResponse>())!;
    }

    public async Task ScanAsync(string accession)
    {
        var res = await _http.PostAsync($"/api/barcodes/scan/{accession}", content: null);
        res.EnsureSuccessStatusCode();
    }
}

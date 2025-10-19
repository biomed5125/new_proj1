using System.Net.Http;
using System.Net.Http.Json;

namespace HMS.CommBench.Services;

public sealed class CommApiClient
{
    private readonly HttpClient _http;
    public CommApiClient(HttpClient http) => _http = http;

    // ---- DTOs (client-side) ----
    public sealed record Row(
        long Id, DateTimeOffset At, long DeviceId, string Direction, string? Type,
        string? AccessionNumber, string? InstrumentTestCode, bool ChecksumOk,
        int Size, string BusinessNo);

    public sealed record PushRequest(string DeviceCode, List<string> Frames);

    // Response shape from POST /api/barcodes/issue
    // Add fields if your API returns more (extra JSON is ignored by the deserializer).
    public sealed record IssueResponse(long LabRequestId, long LabSampleId, string AccessionNumber);

    // Optional strongly-typed request (instead of anonymous object)
    public sealed record IssueRequest(long PatientId, string[] TestCodes);

    // ---- API calls ----
    public Task<List<Row>> GetInboundAsync(long? afterId, int take, CancellationToken ct) =>
        _http.GetFromJsonAsync<List<Row>>($"/api/comm/bench/inbound?afterId={afterId}&take={take}", ct)!;

    public Task<List<Row>> GetOutboundAsync(long? afterId, int take, CancellationToken ct) =>
        _http.GetFromJsonAsync<List<Row>>($"/api/comm/bench/outbound?afterId={afterId}&take={take}", ct)!;

    public Task<HttpResponseMessage> PushAsync(string deviceCode, IEnumerable<string> frames, CancellationToken ct) =>
        _http.PostAsJsonAsync("/api/comm/bench/push", new PushRequest(deviceCode, frames.ToList()), ct);

    public async Task<IssueResponse> IssueLabelAsync(long patientId, string[] testCodes)
    {
        var req = new IssueRequest(patientId, testCodes);
        var res = await _http.PostAsJsonAsync("/api/barcodes/issue", req);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<IssueResponse>())!;
    }

    public async Task ScanAsync(string accession)
    {
        var res = await _http.PostAsync($"/api/barcodes/scan/{accession}", content: null);
        res.EnsureSuccessStatusCode();
    }
}

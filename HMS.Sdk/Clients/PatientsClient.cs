using System.Net.Http.Json;
using HMS.Sdk.Contracts.Patients;

namespace HMS.Sdk.Clients;

public class PatientsClient
{
    private readonly HttpClient _http;
    public PatientsClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<PatientDto>> ListAsync(CancellationToken ct = default) =>
        await _http.GetFromJsonAsync<IReadOnlyList<PatientDto>>("api/patients", ct) ?? Array.Empty<PatientDto>();

    public async Task<PatientDto?> GetAsync(long id, CancellationToken ct = default) =>
        await _http.GetFromJsonAsync<PatientDto>($"api/patients/{id}", ct);

    public async Task<PatientDto?> CreateAsync(object payload, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync("api/patients", payload, ct);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<PatientDto>(cancellationToken: ct);
    }

    public async Task<PatientDto?> UpdateAsync(long id, object payload, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"api/patients/{id}", payload, ct);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<PatientDto>(cancellationToken: ct);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"api/patients/{id}", ct);
        return res.IsSuccessStatusCode;
    }
}

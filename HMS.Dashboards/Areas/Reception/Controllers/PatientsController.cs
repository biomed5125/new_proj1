using HMS.Sdk.Clients;
using HMS.Sdk.Contracts.Patients;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Dashboards.Areas.Reception.Controllers;

[Area("Reception")]
public class PatientsController : Controller
{
    private readonly PatientsClient _client;
    public PatientsController(PatientsClient client) => _client = client;

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
        => View(await _client.ListAsync(ct));

    [HttpGet]
    public IActionResult Create() => View(new CreatePatientVm());

    [HttpPost]
    public async Task<IActionResult> Create(CreatePatientVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);

        var payload = new { firstName = vm.FirstName, lastName = vm.LastName, phone = vm.Phone };
        try
        {
            var created = await _client.CreateAsync(payload, ct);
            TempData["ok"] = $"Created patient #{created?.PatientId}";
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError("", $"API error: {ex.Message}");
            return View(vm);
        }
    }
}

public class CreatePatientVm
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
}

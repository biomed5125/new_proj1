using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HMS.Module.Lab.Infrastructure.Persistence;

namespace HMS.Api.Pages.Lab.Patients;

public sealed class HistoryModel : PageModel
{
    private readonly LabDbContext _db;
    public HistoryModel(LabDbContext db) => _db = db;

    public long Id { get; private set; }
    public string PatientDisplay { get; private set; } = "";

    public async Task OnGet(long id)
    {
        Id = id;
        PatientDisplay = await _db.LabPatients.AsNoTracking()
            .Where(x => x.LabPatientId == id)
            .Select(x => x.FullName +
                         (x.Sex != null ? " / " + x.Sex : "") +
                         (x.DateOfBirth != null ? " / " + x.DateOfBirth!.Value.ToString("yyyy-MM-dd") : ""))
            .FirstOrDefaultAsync() ?? "Patient";
    }
}

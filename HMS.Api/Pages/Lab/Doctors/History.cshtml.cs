using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HMS.Module.Lab.Infrastructure.Persistence;

namespace HMS.Api.Pages.Lab.Doctors;

public sealed class HistoryModel : PageModel
{
    private readonly LabDbContext _db;
    public HistoryModel(LabDbContext db) => _db = db;

    public long Id { get; private set; }
    public string DoctorName { get; private set; } = "Doctor";

    public async Task OnGet(long id)
    {
        Id = id;
        DoctorName = await _db.LabDoctors.AsNoTracking()
            .Where(x => x.LabDoctorId == id)
            .Select(x => x.FullName)
            .FirstOrDefaultAsync() ?? "Doctor";
    }
}

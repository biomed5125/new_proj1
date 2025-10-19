using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HMS.Module.Lab.Infrastructure.Persistence;

namespace HMS.Api.Pages.Lab.Dashboard;

public class OrderModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public long Id { get; set; }

    public VmOrder Details { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync([FromServices] LabDbContext db, CancellationToken ct)
    {
        // read id from ?id= if not bound
        if (Id <= 0 && Request.Query.ContainsKey("id") && long.TryParse(Request.Query["id"], out var qid))
            Id = qid;

        if (Id <= 0) return NotFound();

        // --- header
        var hdr = await db.LabRequests
            .AsNoTracking()
            .Where(r => r.LabRequestId == Id && !r.IsDeleted)
            .Select(r => new
            {
                r.LabRequestId,
                r.OrderNo,
                r.PatientDisplay,
                r.DoctorDisplay,
                r.LabPatientId            // add this
            })
            .SingleOrDefaultAsync(ct);

        if (hdr is null) return NotFound();

        // --- items (query the items table directly; join LabTests for code/name/unit/price)
        var items = await db.LabRequestItems
            .AsNoTracking()
            .Where(i => i.LabRequestId == Id && !i.IsDeleted)
            .OrderBy(i => i.LabRequestItemId)
            .Select(i => new VmItem
            {
                Code = i.LabTest != null ? i.LabTest.Code : null,
                Name = i.LabTest != null ? i.LabTest.Name : null,
                Unit = i.LabTest != null ? i.LabTest.Unit : null,
                Price = i.LabTest != null ? i.LabTest.Price : 0m
            })
            .ToListAsync(ct);

        // --- latest sample (if any)
        var sample = await db.LabSamples
            .AsNoTracking()
            .Where(s => s.LabRequestId == Id && !s.IsDeleted)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new VmSample
            {
                AccessionNumber = s.AccessionNumber,
                Status = s.Status.ToString()
            })
            .FirstOrDefaultAsync(ct);

        Details = new VmOrder
        {
            LabRequestId = hdr.LabRequestId,
            OrderNo = hdr.OrderNo ?? "",
            PatientDisplay = hdr.PatientDisplay,
            DoctorDisplay = hdr.DoctorDisplay,
            LabPatientId = hdr.LabPatientId ?? 0,   // add this line
            Items = items,
            Sample = sample
        };


        return Page();
    }


    public sealed class VmOrder
    {
        public long LabRequestId { get; set; }
        public long LabPatientId { get; set; }
        public string OrderNo { get; set; } = "";
        public string? PatientDisplay { get; set; }
        public string? DoctorDisplay { get; set; }
        public List<VmItem> Items { get; set; } = new();
        public VmSample? Sample { get; set; }
    }
    public sealed class VmItem
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Unit { get; set; }
        public decimal Price { get; set; }
    }
    public sealed class VmSample
    {
        public string? AccessionNumber { get; set; }
        public string Status { get; set; } = "";
    }
}

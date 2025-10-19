using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HMS.Api.Pages.Lab.Labels
{
    public class AccessionModel : PageModel
    {
        private readonly LabDbContext _db;
        public AccessionModel(LabDbContext db) => _db = db;

        [Microsoft.AspNetCore.Mvc.FromRoute] public string Accession { get; set; } = default!;
        public myLabSample? Sample { get; private set; }

        public async Task OnGetAsync(string accession)
        {
            Accession = accession;

            // Always read fresh including items for the preview
            Sample = await _db.LabSamples
                .Include(s => s.Request)
                    .ThenInclude(r => r.Items.Where(i => !i.IsDeleted))
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.AccessionNumber == accession);

            // Mark printed (for traceability); you can show a "Reprint" badge if it was false before
            var s = await _db.LabSamples.FirstOrDefaultAsync(x => x.AccessionNumber == accession);
            if (s != null && !s.LabelPrinted)
            {
                s.LabelPrinted = true;
                s.UpdatedAt = DateTime.UtcNow;
                s.UpdatedBy = "label";
                await _db.SaveChangesAsync();
            }
        }
    }

}


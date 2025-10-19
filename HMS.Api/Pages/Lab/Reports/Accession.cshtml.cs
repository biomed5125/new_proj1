using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HMS.Api.Pages.Lab.Reports
{
    public sealed class AccessionModel : PageModel
    {
        private readonly LabDbContext _db;
        public AccessionModel(LabDbContext db) => _db = db;

        // existing props...
        public string AccessionNumber { get; private set; } = "";
        public myLabPreanalytical? Preanalytical { get; private set; }
        public List<string> PreanalyticalSummary { get; private set; } = new();

        public async Task OnGet(string acc)
        {
            AccessionNumber = acc;

            // find requestId by accession
            var lrid = await _db.LabSamples.AsNoTracking()
                .Where(s => s.AccessionNumber == acc)
                .Select(s => (long?)s.LabRequestId)
                .FirstOrDefaultAsync();

            if (lrid is null) return;

            Preanalytical = await _db.Set<myLabPreanalytical>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.LabRequestId == lrid.Value);

            if (Preanalytical is not null)
            {
                var x = Preanalytical;
                void add(string s) { if (!string.IsNullOrWhiteSpace(s)) PreanalyticalSummary.Add(s); }

                if (x.IsDiabetic) add("Diabetic");
                if (x.TookAntibioticLast3Days) add("Antibiotic (≤3d)");
                if (x.FastingHours is not null) add($"Fasting {x.FastingHours}h");
                if (!string.IsNullOrWhiteSpace(x.ThyroidStatus) && x.ThyroidStatus != "None") add($"{x.ThyroidStatus} thyroid");
                if (x.HasAllergy) add("Allergy");
                if (x.HasAnemia) add("Anemia");
                if (x.HasFattyLiver) add("Fatty liver");
                if (x.HasHighCholesterol) add("High cholesterol");
                if (x.Dialysis) add("Dialysis");
                if (x.CardiacAttackHistory) add("Cardiac attack (hx)");
                if (x.Pacemaker) add("Pacemaker");
                if (x.BloodPressureSys is not null && x.BloodPressureDia is not null) add($"BP {x.BloodPressureSys}/{x.BloodPressureDia}");
                if (x.PulseBpm is not null) add($"Pulse {x.PulseBpm}");
            }

            // ...load the rest of your report data as you already do
        }
    }
}
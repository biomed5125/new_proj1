using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HMS.Module.Lab.Infrastructure.Persistence;
using HMS.Module.Lab.Features.Lab.Models.Entities;

namespace HMS.Api.Pages.Lab.Patients
{
    public class IndexModel : PageModel
    {
        private readonly LabDbContext _db;
        public IndexModel(LabDbContext db) => _db = db;

        public List<Row> Items { get; private set; } = new();

        [BindProperty] public FormDto Form { get; set; } = new();

        public async Task OnGetAsync()
        {
            Items = await _db.LabPatients
                .AsNoTracking()
                .OrderByDescending(x => x.LabPatientId)
                .Take(200)
                .Select(x => new Row
                {
                    LabPatientId = x.LabPatientId,
                    FullName = x.FullName,
                    MRN = x.Mrn,
                    DateOfBirth = x.DateOfBirth,
                    Sex = x.Sex,
                    Phone = x.Phone,
                    Email = x.Email
                }).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) { await OnGetAsync(); return Page(); }

            _db.LabPatients.Add(new myLabPatient
            {
                FullName = Form.FullName.Trim(),
                Mrn = string.IsNullOrWhiteSpace(Form.MRN) ? null : Form.MRN.Trim(),
                DateOfBirth = Form.DateOfBirth,
                Sex = string.IsNullOrWhiteSpace(Form.Sex) ? null : Form.Sex,
                Phone = string.IsNullOrWhiteSpace(Form.Phone) ? null : Form.Phone,
                Email = string.IsNullOrWhiteSpace(Form.Email) ? null : Form.Email
            });
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }

        public sealed class FormDto
        {
            [Required, StringLength(160)]
            public string FullName { get; set; } = "";
            [StringLength(40)]
            public string? MRN { get; set; }
            [DataType(DataType.Date)]
            public DateTime? DateOfBirth { get; set; }
            [StringLength(1)]
            public string? Sex { get; set; }
            [StringLength(40)]
            public string? Phone { get; set; }
            [StringLength(80)]
            public string? Email { get; set; }
        }

        public sealed class Row
        {
            public long LabPatientId { get; set; }
            public string? FullName { get; set; }
            public string? MRN { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string? Sex { get; set; }
            public string? Phone { get; set; }
            public string? Email { get; set; }
        }
    }
}

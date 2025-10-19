using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HMS.Module.Lab.Infrastructure.Persistence;
using HMS.Module.Lab.Features.Lab.Models.Entities;

namespace HMS.Api.Pages.Lab.Doctors
{
    public class IndexModel : PageModel
    {
        private readonly LabDbContext _db;
        public IndexModel(LabDbContext db) => _db = db;

        public List<Row> Items { get; private set; } = new();

        [BindProperty] public FormDto Form { get; set; } = new();

        public async Task OnGetAsync()
        {
            Items = await _db.LabDoctors
                .AsNoTracking()
                .OrderByDescending(x => x.LabDoctorId)
                .Take(200)
                .Select(x => new Row
                {
                    LabDoctorId = x.LabDoctorId,
                    FullName = x.FullName,
                    LicenseNo = x.LicenseNo,
                    Phone = x.Phone,
                    Email = x.Email,
                    Speciality=x.Specialty
                }).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) { await OnGetAsync(); return Page(); }

            _db.LabDoctors.Add(new myLabDoctor
            {
                FullName = Form.FullName.Trim(),
                LicenseNo = string.IsNullOrWhiteSpace(Form.LicenseNo) ? null : Form.LicenseNo.Trim(),
                Phone = string.IsNullOrWhiteSpace(Form.Phone) ? null : Form.Phone,
                Email = string.IsNullOrWhiteSpace(Form.Email) ? null : Form.Email,
                Specialty=string.IsNullOrWhiteSpace(Form.Speciality)?null:Form.Speciality.Trim(),
            });
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }

        public sealed class FormDto
        {
            [Required, StringLength(160)]
            public string FullName { get; set; } = "";
            [StringLength(40)]
            public string? LicenseNo { get; set; }
            [StringLength(40)]
            public string? Phone { get; set; }
            [StringLength(160), EmailAddress]
            public string? Email { get; set; }
            [StringLength(60)]
            public string? Speciality { get; set; }
        }

        public sealed class Row
        {
            public long LabDoctorId { get; set; }
            public string? FullName { get; set; }
            public string? LicenseNo { get; set; }
            public string? Phone { get; set; }
            public string? Email { get; set; }
            public string? Speciality{get;set;}
        }
    }
}

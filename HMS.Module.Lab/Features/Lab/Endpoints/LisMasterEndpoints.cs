using HMS.Module.Lab.Features.Lab.Models.Dtos;
using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Lab.Features.Lab.Endpoints;

public static class LisMasterEndpoints
{
    public static IEndpointRouteBuilder MapLisMasters(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/lab/lis").WithTags("LIS");

        // Patients
        g.MapGet("/patients", async (LabDbContext db, CancellationToken ct) =>
            await db.LabPatients.AsNoTracking().OrderBy(x => x.FullName)
                .Select(p => new LabPatientDto(p.LabPatientId, p.FullName, p.Mrn, p.DateOfBirth, p.Sex, p.Phone, p.Email))
                .ToListAsync(ct));

        g.MapPost("/patients", async ([FromBody] UpsertPatientDto dto, LabDbContext db, CancellationToken ct) =>
        {
            var p = new myLabPatient
            {
                FullName = dto.FullName.Trim(),
                Mrn = dto.Mrn,
                DateOfBirth = dto.DateOfBirth,
                Sex = dto.Sex,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "ui"
            };
            db.LabPatients.Add(p);
            await db.SaveChangesAsync(ct);
            return Results.Ok(new { p.LabPatientId, p.FullName });
        });

        // Doctors
        g.MapGet("/doctors", async (LabDbContext db, CancellationToken ct) =>
            await db.LabDoctors.AsNoTracking().OrderBy(x => x.FullName)
                .Select(d => new LabDoctorDto(d.LabDoctorId, d.FullName, d.LicenseNo, d.Phone, d.Email))
                .ToListAsync(ct));

        g.MapPost("/doctors", async ([FromBody] UpsertDoctorDto dto, LabDbContext db, CancellationToken ct) =>
        {
            var d = new myLabDoctor
            {
                FullName = dto.FullName.Trim(),
                LicenseNo = dto.LicenseNo,
                Phone = dto.Phone,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "ui"
            };
            db.LabDoctors.Add(d);
            await db.SaveChangesAsync(ct);
            return Results.Ok(new { d.LabDoctorId, d.FullName });
        });

        return app;
    }
}

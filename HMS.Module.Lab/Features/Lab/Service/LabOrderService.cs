//using HMS.Module.Lab.Features.Lab.Infrastructure;
using HMS.Module.Lab.Features.Lab.Models.Dtos;
using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Features.Lab.Models.Enums;
using HMS.Module.Lab.Infrastructure.Persistence;
using HMS.SharedKernel.Ids;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace HMS.Module.Lab.Service;
public sealed class LabOrderService : ILabOrderService
{
    private readonly LabDbContext _db;
    private readonly IBusinessIdGenerator _ids;
    public LabOrderService(LabDbContext db, IBusinessIdGenerator ids) { _db = db; _ids = ids; }

    public async Task<long> CreateRequestAsync(CreateLabRequestDto dto, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var orderNo = _ids.NewLabOrderNo(now);

        // expand panels into test list
        var tests = new HashSet<long>(dto.TestIds ?? Array.Empty<long>());
        if (dto.PanelIds is { Count: > 0 })
        {
            var pTests = await _db.LabPanelItems
                .Where(pi => dto.PanelIds.Contains(pi.LabPanelId))
                .Select(pi => pi.LabTestId).ToListAsync(ct);
            foreach (var t in pTests) tests.Add(t);
        }
        if (tests.Count == 0) throw new InvalidOperationException("No tests provided.");

        var req = new myLabRequest
        {
            PatientId = dto.PatientId,
            AdmissionId = dto.AdmissionId,
            DoctorId = dto.DoctorId,
            Priority = string.IsNullOrWhiteSpace(dto.Priority) ? "Routine" : dto.Priority,
            Status = LabRequestStatus.Requested,
            Notes = dto.Notes,
            OrderNo = orderNo,
            CreatedAt = now,
            CreatedBy = "api"
        };
        _db.LabRequests.Add(req);
        await _db.SaveChangesAsync(ct); // assigns HiLo key

        foreach (var testId in tests)
            _db.LabRequestItems.Add(new myLabRequestItem
            {
                LabRequestId = req.LabRequestId,
                LabTestId = testId,
                CreatedAt = now,
                CreatedBy = "api"
            });
        // prefetch tests once to populate code/name/unit on items
        var testInfos = await _db.LabTests
                    .Where(t => tests.Contains(t.LabTestId))
                    .Select(t => new { t.LabTestId, t.Code, t.Name, t.Unit })
                    .ToDictionaryAsync(t => t.LabTestId, t => t, ct);

        foreach (var testId in tests)
        {
            var t = testInfos[testId];
            _db.LabRequestItems.Add(new myLabRequestItem
            {
                LabRequestId = req.LabRequestId,
                LabTestId = testId,
                LabTestCode = t.Code,
                LabTestName = t.Name,
                LabTestUnit = t.Unit,
                CreatedAt = now,
                CreatedBy = "api"
            });
        }
        await _db.SaveChangesAsync(ct);
        return req.LabRequestId;
    }
}

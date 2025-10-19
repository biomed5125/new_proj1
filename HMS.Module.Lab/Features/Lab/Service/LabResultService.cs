//using HMS.Module.Lab.Features.Lab.Infrastructure;
using HMS.Module.Lab.Features.Lab.Models.Dtos;
using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Features.Lab.Models.Enums;
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Lab.Service;
public sealed class LabResultService : ILabResultService
{
    private readonly LabDbContext _db;
    public LabResultService(LabDbContext db) => _db = db;

    public async Task EnterAsync(IEnumerable<EnterResultDto> lines, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        foreach (var l in lines)
        {
            var item = await _db.LabRequestItems
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.LabRequestItemId == l.LabRequestItemId, ct)
                ?? throw new InvalidOperationException($"RequestItem {l.LabRequestItemId} not found");

            var test = await _db.LabTests.Include(t => t.DefaultReferenceRange)
                .FirstAsync(t => t.LabTestId == item.LabTestId, ct);

            // compute flag
            ResultFlag? flag = null;
            if (decimal.TryParse(l.Value, out var val) && test.DefaultReferenceRange is not null)
            {
                var lo = test.DefaultReferenceRange.RefLow;
                var hi = test.DefaultReferenceRange.RefHigh;
                if (lo.HasValue && val < lo) flag = ResultFlag.Low;
                else if (hi.HasValue && val > hi) flag = ResultFlag.High;
                else flag = ResultFlag.Normal;
            }

            var existing = await _db.LabResults.FirstOrDefaultAsync(x => x.LabRequestItemId == l.LabRequestItemId, ct);
            if (existing == null)
            {
                _db.LabResults.Add(new myLabResult
                {
                    LabRequestId = item.LabRequestId,
                    LabRequestItemId = item.LabRequestItemId,
                    Value = l.Value,
                    Unit = l.Unit ?? test.Unit,
                    RefLow = test.DefaultReferenceRange?.RefLow,
                    RefHigh = test.DefaultReferenceRange?.RefHigh,
                    Flag = flag,
                    Status = LabResultStatus.Entered,
                    CreatedAt = now,
                    CreatedBy = "api"
                });
            }
            else
            {
                existing.Value = l.Value;
                existing.Unit = l.Unit ?? test.Unit;
                existing.RefLow = test.DefaultReferenceRange?.RefLow;
                existing.RefHigh = test.DefaultReferenceRange?.RefHigh;
                existing.Flag = flag;
                existing.Status = LabResultStatus.Entered;
                existing.UpdatedAt = now; existing.UpdatedBy = "api";
            }
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task<bool> ApproveAsync(long labRequestId, long approvedByDoctorId, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var req = await _db.LabRequests.FirstOrDefaultAsync(x => x.LabRequestId == labRequestId, ct);
        if (req == null) return false;

        var results = await _db.LabResults.Where(r => r.LabRequestId == labRequestId).ToListAsync(ct);
        foreach (var r in results)
        {
            r.Status = LabResultStatus.Final;
            r.ApprovedByDoctorId = approvedByDoctorId;
            r.ApprovedAtUtc = now;
        }
        req.Status = LabRequestStatus.Completed; req.UpdatedAt = now;
        await _db.SaveChangesAsync(ct);
        return true;
    }
}

using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Features.Lab.Models.Enums;
using HMS.Module.Lab.Infrastructure.Persistence;
using HMS.SharedKernel.Ids;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Lab.Service;

public sealed class LabSampleService : ILabSampleService
{
    private readonly LabDbContext _db;
    private readonly IBusinessIdGenerator _ids;
    public LabSampleService(LabDbContext db, IBusinessIdGenerator ids) { _db = db; _ids = ids; }

    /// <summary>
    /// Idempotent: if a sample already exists for this request, returns its id.
    /// Otherwise creates it and (optionally) transitions request status.
    /// </summary>
    public async Task<long> CollectAsync(long labRequestId, string collector, CancellationToken ct)
    {
        var req = await _db.LabRequests
            .FirstOrDefaultAsync(x => x.LabRequestId == labRequestId, ct);
        if (req is null) throw new InvalidOperationException("Request not found");

        // If a sample already exists, just return it (prevents unique index errors and double rows)
        var existing = await _db.LabSamples
            .FirstOrDefaultAsync(s => s.LabRequestId == labRequestId && !s.IsDeleted, ct);
        if (existing is not null)
        {
            // Make sure it is at least Collected
            if (existing.Status == LabSampleStatus.Collected || existing.Status == LabSampleStatus.Labeled)
                return existing.LabSampleId;
            // If it somehow was created differently, normalize it
            existing.Status = LabSampleStatus.Collected;
            existing.CollectedAtUtc ??= DateTime.UtcNow;
            existing.CollectedBy ??= collector;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = collector;
            await _db.SaveChangesAsync(ct);
            return existing.LabSampleId;
        }

        // Normal first-time collect
        if (req.Status != LabRequestStatus.Requested && req.Status != LabRequestStatus.Completed)
            throw new InvalidOperationException("Bad status");

        var when = DateTime.UtcNow;
        var accession = _ids.NewAccessionNumber(when);

        var s = new myLabSample
        {
            LabRequestId = labRequestId,
            AccessionNumber = accession,
            Status = LabSampleStatus.Collected,
            CollectedAtUtc = when,
            CollectedBy = collector,
            LabelPrinted = false,        // default
            CreatedAt = when,
            CreatedBy = collector
        };
        _db.LabSamples.Add(s);

        // keep request “Completed” after collection (as you did)
        req.Status = LabRequestStatus.Completed;
        req.UpdatedAt = when;
        req.UpdatedBy = collector;

        await _db.SaveChangesAsync(ct);
        return s.LabSampleId;
    }



    /// <summary>
    /// Idempotent: receiving an already received sample returns true without error.
    /// </summary>
    public async Task<bool> ReceiveAsync(long labSampleId, string receiver, CancellationToken ct)
    {
        var s = await _db.LabSamples.FirstOrDefaultAsync(x => x.LabSampleId == labSampleId, ct);
        if (s is null) return false;

        if (s.Status != LabSampleStatus.Received)
        {
            s.Status = LabSampleStatus.Received;
            s.ReceivedAtUtc = DateTime.UtcNow;
            s.ReceivedBy = receiver;
            s.UpdatedAt = DateTime.UtcNow;
            s.UpdatedBy = receiver;
            await _db.SaveChangesAsync(ct);
        }
        return true;
    }

}

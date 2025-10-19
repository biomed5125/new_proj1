using HMS.Module.Lab.Features.Lab.Models.Enums;
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Module.Lab.Features.Lab.Dashboard;

public interface ILabDashboardService
{
    Task<IReadOnlyList<DashOrderRowDto>> GetOrdersAsync(CancellationToken ct);
    Task<DashOrderDetailsDto?> GetOrderAsync(long labRequestId, CancellationToken ct);

    Task<IReadOnlyList<DashSampleRowDto>> GetPendingSamplesAsync(CancellationToken ct);

    Task<IReadOnlyList<DashResultRowDto>> GetResultsAsync(CancellationToken ct);
    Task<IReadOnlyList<ResultHistoryDto>> GetResultHistoryAsync(long labRequestItemId, CancellationToken ct);

    Task<SummaryDto> GetSummaryAsync(CancellationToken ct);
}

public sealed class LabDashboardService : ILabDashboardService
{
    private readonly LabDbContext _db;
    public LabDashboardService(LabDbContext db) => _db = db;

    // ---------------------------------------------------------------------
    // ORDERS (LIST)
    // ---------------------------------------------------------------------
    public async Task<IReadOnlyList<DashOrderRowDto>> GetOrdersAsync(CancellationToken ct)
    {
        // headers
        var orders = await _db.LabRequests.AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new { x.LabRequestId, x.OrderNo, x.PatientId, x.CreatedAt })
            .Take(200)
            .ToListAsync(ct);

        if (orders.Count == 0) return Array.Empty<DashOrderRowDto>();

        var reqIds = orders.Select(o => o.LabRequestId).ToArray();

        // item counts per request (for status)
        var itemCounts = await _db.LabRequestItems.AsNoTracking()
            .Where(i => reqIds.Contains(i.LabRequestId))
            .GroupBy(i => i.LabRequestId)
            .Select(g => new { LabRequestId = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var itemCountByReq = itemCounts.ToDictionary(x => x.LabRequestId, x => x.Count);

        // codes per request (join items->tests)
        var codes = await (
            from i in _db.LabRequestItems.AsNoTracking()
            join t in _db.LabTests.AsNoTracking() on i.LabTestId equals t.LabTestId
            where reqIds.Contains(i.LabRequestId)
            select new { i.LabRequestId, t.Code }
        ).ToListAsync(ct);

        var testsByReq = codes
            .GroupBy(x => x.LabRequestId)
            .ToDictionary(g => g.Key, g => string.Join(',', g.Select(x => x.Code ?? "").Where(s => !string.IsNullOrWhiteSpace(s)).Distinct()));

        // result counts per request
        var resultCounts = await _db.LabResults.AsNoTracking()
            .Where(r => reqIds.Contains(r.LabRequestId))
            .GroupBy(r => r.LabRequestId)
            .Select(g => new { LabRequestId = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var resByReq = resultCounts.ToDictionary(x => x.LabRequestId, x => x.Count);

        // build rows
        // In GetOrdersAsync(...)
        var rows = await _db.LabRequests
            .AsNoTracking()
            .Where(r => !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .Take(200)
            .Select(r => new DashOrderRowDto(
                r.LabRequestId,
                r.OrderNo,
                r.PatientDisplay,
                r.LabPatientId != null
                    ? _db.LabPatients.Where(p => p.LabPatientId == r.LabPatientId).Select(p => p.Mrn).FirstOrDefault()
                    : null,
                string.Join(",", r.Items.Where(i => !i.IsDeleted)
                                        .OrderBy(i => i.LabRequestItemId)
                                        .Select(i => i.LabTestCode ?? (i.LabTest != null ? i.LabTest.Code : ""))),
                // status synthesis
                r.Items.Any()
                    ? (r.Items.All(i => i.Results.Any(x => x.Status == Models.Enums.LabResultStatus.Final)) ? "Final" :
                       r.Items.Any(i => i.Results.Any()) ? "Partial" : "Requested")
                    : "Requested",
                (r.UpdatedAt ?? r.CreatedAt).ToLocalTime().ToString("g"),
                r.Source))
            .ToListAsync(ct);
        return rows;

    }

    // ---------------------------------------------------------------------
    // ORDER (DETAIL)
    // ---------------------------------------------------------------------
    public async Task<DashOrderDetailsDto?> GetOrderAsync(long labRequestId, CancellationToken ct)
    {
        var header = await _db.LabRequests.AsNoTracking()
            .Where(x => x.LabRequestId == labRequestId)
            .Select(x => new { x.LabRequestId, x.OrderNo, x.PatientId })
            .FirstOrDefaultAsync(ct);

        if (header is null) return null;

        var items = await (
            from i in _db.LabRequestItems.AsNoTracking()
            join t in _db.LabTests.AsNoTracking() on i.LabTestId equals t.LabTestId
            where i.LabRequestId == labRequestId
            orderby t.Code
            select new DashOrderItem(
                i.LabRequestItemId,
                t.Code ?? "",
                t.Name ?? "",
                t.Unit ?? "",
                t.Price
            )
        ).ToListAsync(ct);

        var sample = await _db.LabSamples.AsNoTracking()
            .Where(s => s.LabRequestId == labRequestId)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new DashSampleDto(
                s.LabSampleId,
                s.AccessionNumber ?? "",
                s.Status,
                s.CreatedAt
            ))
            .FirstOrDefaultAsync(ct);

        return new DashOrderDetailsDto(
            header.LabRequestId,
            header.OrderNo ?? "",
            header.PatientId,
            items,
            sample
        );
    }

    // ---------------------------------------------------------------------
    // SAMPLES (PENDING)
    // ---------------------------------------------------------------------
    public async Task<IReadOnlyList<DashSampleRowDto>> GetPendingSamplesAsync(CancellationToken ct)
    {
        // Treat anything not Received as "pending"
        var rows = await _db.LabSamples.AsNoTracking()
            .Where(s => s.Status != LabSampleStatus.Received)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new DashSampleRowDto(
                s.LabSampleId,
                s.LabRequestId,
                s.AccessionNumber ?? "",
                s.Status,
                s.CreatedAt
            ))
            .Take(100)
            .ToListAsync(ct);

        return rows;
    }

    // ---------------------------------------------------------------------
    // RESULTS (LIST)
    // ---------------------------------------------------------------------
    public async Task<IReadOnlyList<DashResultRowDto>> GetResultsAsync(CancellationToken ct)
    {
        var q =
            from r in _db.LabResults.AsNoTracking()
            join i in _db.LabRequestItems.AsNoTracking() on r.LabRequestItemId equals i.LabRequestItemId
            join t in _db.LabTests.AsNoTracking() on i.LabTestId equals t.LabTestId
            join s in _db.LabSamples.AsNoTracking() on r.LabRequestId equals s.LabRequestId into sg
            from s in sg.DefaultIfEmpty()
            orderby (r.UpdatedAt ?? r.CreatedAt) descending
            select new
            {
                resultId = r.LabResultId,
                requestId = r.LabRequestId,
                requestItemId = r.LabRequestItemId,
                code = t.Code,
                name = t.Name,
                value = r.Value,
                unit = r.Unit,
                flag = r.Flag,
                status = r.Status,
                accessionNo = r.AccessionNumber ?? s.AccessionNumber,
                createdAtUtc = r.CreatedAt,
                updatedAtUtc = r.UpdatedAt
            };

        var rows = await q.Take(50).ToListAsync(ct);

        return rows
            .Select(x => new DashResultRowDto(
                x.resultId,
                x.requestId,
                x.requestItemId,
                x.code ?? "",
                x.name ?? "",
                x.value ?? "",
                x.unit ?? "",
                x.flag,
                x.status,
                x.accessionNo ?? "",
                x.createdAtUtc,
                x.updatedAtUtc
            ))
            .ToList();
    }

    // ---------------------------------------------------------------------
    // RESULT HISTORY (PER REQUEST ITEM)
    // ---------------------------------------------------------------------
    public async Task<IReadOnlyList<ResultHistoryDto>> GetResultHistoryAsync(long labRequestItemId, CancellationToken ct)
    {
        var q =
            from r in _db.LabResults.AsNoTracking()
            where r.LabRequestItemId == labRequestItemId
            orderby (r.UpdatedAt ?? r.CreatedAt)
            select new
            {
                atUtc = r.UpdatedAt ?? r.CreatedAt,
                value = r.Value,
                unit = r.Unit,
                flag = r.Flag,
                status = r.Status,
                accessionNo = r.AccessionNumber
            };

        var rows = await q.ToListAsync(ct);

        return rows
            .Select(x => new ResultHistoryDto(
                x.atUtc,
                x.value ?? "",
                x.unit ?? "",
                x.flag,
                x.status,
                x.accessionNo ?? ""
            ))
            .ToList();
    }

    // ---------------------------------------------------------------------
    // SUMMARY
    // ---------------------------------------------------------------------
    public async Task<SummaryDto> GetSummaryAsync(CancellationToken ct)
    {
        var totalOrders = await _db.LabRequests.AsNoTracking().CountAsync(ct);
        var pendingSamples = await _db.LabSamples.AsNoTracking().CountAsync(s => s.Status != LabSampleStatus.Received, ct);
        var totalResults = await _db.LabResults.AsNoTracking().CountAsync(ct);
        var finalResults = await _db.LabResults.AsNoTracking().CountAsync(r => r.Status == LabResultStatus.Final, ct);

        return new SummaryDto(totalOrders, pendingSamples, totalResults, finalResults);
    }
}

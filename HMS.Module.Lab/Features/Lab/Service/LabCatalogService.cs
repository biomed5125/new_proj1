//using HMS.Module.Lab.Features.Lab.Infrastructure;
using HMS.Module.Lab.Features.Lab.Models.Dtos;
using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Lab.Service;
public sealed class LabCatalogService : ILabCatalogService
{
    private readonly LabDbContext _db;
    public LabCatalogService(LabDbContext db) => _db = db;

    public async Task<myLabTest> UpsertTestAsync(UpsertTestDto dto, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var rr = dto.RefLow.HasValue || dto.RefHigh.HasValue
                 ? new myReferenceRange { RefLow = dto.RefLow, RefHigh = dto.RefHigh, CreatedAt = now, CreatedBy = "api" }
                 : null;

        if (rr != null) _db.ReferenceRanges.Add(rr);

        var existing = await _db.LabTests.FirstOrDefaultAsync(x => x.Code == dto.Code, ct);
        if (existing == null)
        {
            var t = new myLabTest
            {
                Code = dto.Code,
                Name = dto.Name,
                SpecimenTypeId = dto.SpecimenTypeId,
                Unit = dto.Unit,
                Price = dto.Price,
                TatMinutes = dto.TatMinutes,
                IsActive = dto.IsActive,
                DefaultReferenceRange = rr,
                CreatedAt = now,
                CreatedBy = "api"
            };
            _db.LabTests.Add(t);
            await _db.SaveChangesAsync(ct);
            return t;
        }
        existing.Name = dto.Name; existing.SpecimenTypeId = dto.SpecimenTypeId; existing.Unit = dto.Unit;
        existing.Price = dto.Price; existing.TatMinutes = dto.TatMinutes; existing.IsActive = dto.IsActive;
        if (rr != null) existing.DefaultReferenceRange = rr;
        existing.UpdatedAt = now; existing.UpdatedBy = "api";
        await _db.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<myLabPanel> UpsertPanelAsync(UpsertPanelDto dto, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var p = await _db.LabPanels.Include(x => x.Items).FirstOrDefaultAsync(x => x.Code == dto.Code, ct);
        if (p == null)
        {
            p = new myLabPanel { Code = dto.Code, Name = dto.Name, IsActive = dto.IsActive, CreatedAt = now, CreatedBy = "api" };
            _db.LabPanels.Add(p);
        }
        else
        {
            p.Name = dto.Name; p.IsActive = dto.IsActive; p.UpdatedAt = now; p.UpdatedBy = "api";
            // reset items
            _db.LabPanelItems.RemoveRange(p.Items);
            p.Items.Clear();
        }
        // add items
        int i = 0;
        foreach (var id in dto.TestIds.Distinct()) p.Items.Add(new myLabPanelItem { LabTestId = id, SortOrder = i++ });

        await _db.SaveChangesAsync(ct);
        return p;
    }
    // Features/Lab/Service/LabCatalogService.cs  (ADD implementations – keep your existing code)
    public async Task<myLabTest?> GetTestAsync(long id, CancellationToken ct)
        => await _db.LabTests
            .Include(t => t.DefaultReferenceRange)
            .FirstOrDefaultAsync(t => t.LabTestId == id, ct);

    public async Task<bool> DeleteTestAsync(long id, CancellationToken ct)
    {
        var entity = await _db.LabTests.FirstOrDefaultAsync(x => x.LabTestId == id, ct);
        if (entity is null) return false;

        var isReferenced = await _db.LabRequestItems.AnyAsync(i => i.LabTestId == id && !i.IsDeleted, ct);
        if (isReferenced)
        {
            entity.IsActive = false;         // soft retire
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = "api";
        }
        else
        {
            _db.LabTests.Remove(entity);
        }
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> ToggleTestAsync(long id, bool active, CancellationToken ct)
    {
        var t = await _db.LabTests.FirstOrDefaultAsync(x => x.LabTestId == id, ct);
        if (t is null) return false;
        t.IsActive = active;
        t.UpdatedAt = DateTime.UtcNow;
        t.UpdatedBy = "api";
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<myLabPanel?> GetPanelAsync(long id, CancellationToken ct)
        => await _db.LabPanels.Include(p => p.Items).FirstOrDefaultAsync(p => p.LabPanelId == id, ct);

    public async Task<bool> DeletePanelAsync(long id, CancellationToken ct)
    {
        var p = await _db.LabPanels.Include(x => x.Items).FirstOrDefaultAsync(x => x.LabPanelId == id, ct);
        if (p is null) return false;

        if (await _db.LabRequestItems.AnyAsync(i => p.Items.Select(it => it.LabTestId).Contains(i.LabTestId) && !i.IsDeleted, ct))
        {
            p.IsActive = false;
            p.IsDeleted = true;
            p.UpdatedAt = DateTime.UtcNow;
            p.UpdatedBy = "api";
        }
        else
        {
            _db.LabPanelItems.RemoveRange(p.Items);
            _db.LabPanels.Remove(p);
        }
        await _db.SaveChangesAsync(ct);
        return true;
    }

}

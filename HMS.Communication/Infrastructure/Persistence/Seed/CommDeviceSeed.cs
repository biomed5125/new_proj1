// HMS.Communication.Infrastructure/Persistence/Seed/CommDeviceSeed.cs
using HMS.Communication.Domain.Entities;
using HMS.Communication.Infrastructure.Persistence.Entities;
using HMS.Communication.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Communication.Infrastructure.Persistence.Seed;

public static class CommDeviceSeed
{
    public static async Task EnsureAsync(CommunicationDbContext db, CancellationToken ct = default)
    {
        async Task<long> ProfileId(string name)
            => await db.AnalyzerProfiles
                .Where(p => p.Name == name)
                .Select(p => p.AnalyzerProfileId)
                .FirstAsync(ct);

        async Task Ensure(string code, string name, string profile, string? port = null)
        {
            if (await db.CommDevices.AnyAsync(d => d.DeviceCode == code, ct)) return;

            db.CommDevices.Add(new CommDevice
            {
                DeviceCode = code,
                Name = name,
                AnalyzerProfileId = await ProfileId(profile),
                PortName = port,
                IsActive = true
            });
        }

        // --- Roche (existing) ---
        await Ensure("COBAS-C311-01", "cobas c311 (Chem)", "Roche Cobas c311", "COM1");
        await Ensure("COBAS-E411-01", "cobas e411 (Immuno)", "Roche Cobas e411", "COM2");
        await Ensure("COBAS-C501-01", "cobas c501 (Chem)", "Roche Cobas c311", "COM3");
        await Ensure("COBAS-E601-01", "cobas e601 (Immuno)", "Roche Cobas e411", "COM4");
        await Ensure("ROCHE1", "Demo cobas e411", "Roche Cobas e411", "COM11");

        // --- Sysmex XP-300 (SUIT) ---
        await Ensure("SYSMEX-XP300-01", "Sysmex XP-300 (CBC)", "Sysmex XP-300", "COM6");

        // --- Fuji Dri-Chem NX500 (ASCII) ---
        await Ensure("FUJI-NX500-01", "Fuji Dri-Chem NX500", "Fuji Dri-Chem NX500", "COM7");

        await db.SaveChangesAsync(ct);
    }
}

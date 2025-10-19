//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using HMS.Communication.Domain.Entities;
//using HMS.Communication.Domain.ValueObjects;
//using HMS.Communication.Infrastructure.Persistence;
//using HMS.Module.Lab.Infrastructure.Persistence;
//using HMS.Module.Lab.Features.Lab.Models.Entities;
//using HMS.Module.Lab.Features.Lab.Models.Enums;
//using System.IO.Ports;
//using HMS.Communication.Persistence;
//using HMS.Communication.Infrastructure.Persistence.Entities;

//namespace HMS.Api.CompositionRoot;

//public static class DevSeedHelpers
//{
//    // STEP 2
//    public static async Task SeedCommDemoAsync(IServiceProvider root, CancellationToken ct = default)
//    {
//        await using var scope = root.CreateAsyncScope();
//        var sp = scope.ServiceProvider;
//        var db = sp.GetRequiredService<CommunicationDbContext>();

//        if (!await db.Devices.AsNoTracking().AnyAsync(ct))
//        {
//            db.Devices.Add(new CommDevice
//            {
//                DeviceCode = "ROCHE1",
//                Name = "Roche cobas e 411 (demo)",
//                IsEnabled = true,
//                Options = new DeviceOptions
//                {
//                    DeviceCode = "ROCHE1",
//                    Manufacturer = "Roche",
//                    Model = "cobas e 411",
//                    Serial = new SerialSettings
//                    {
//                        PortName = "COM3",
//                        BaudRate = 9600,
//                        DataBits = 8,
//                        Parity = Parity.None,
//                        StopBits = StopBits.One,
//                        Handshake = Handshake.None
//                    }
//                },
//                CreatedAt = DateTime.UtcNow,
//                CreatedBy = "seed"
//            });

//            await db.SaveChangesAsync(ct);
//        }
//    }

//    // STEP 3
//    public static async Task SeedLabInstrumentMapAsync(IServiceProvider root, CancellationToken ct = default)
//    {
//        await using var scope = root.CreateAsyncScope();
//        var sp = scope.ServiceProvider;
//        var lab = sp.GetRequiredService<LabDbContext>();
//        var comm = sp.GetRequiredService<CommunicationDbContext>();

//        var deviceId = await comm.Devices
//            .Where(d => d.DeviceCode == "ROCHE1")
//            .Select(d => d.Id)
//            .FirstAsync(ct);

//        async Task EnsureAsync(string instCode, string lisCode)
//        {
//            var testId = await lab.LabTests
//                .Where(t => t.Code == lisCode)
//                .Select(t => t.LabTestId)
//                .FirstAsync(ct);

//            var exists = await lab.InstrumentTestMaps
//                .AnyAsync(m => m.DeviceId == deviceId && m.InstrumentTestCode == instCode, ct);

//            if (!exists)
//            {
//                lab.InstrumentTestMaps.Add(new myInstrumentTestMap
//                {
//                    DeviceId = deviceId,
//                    LabTestId = testId,
//                    InstrumentTestCode = instCode,
//                    CreatedAt = DateTime.UtcNow,
//                    CreatedBy = "seed"
//                });
//                await lab.SaveChangesAsync(ct);
//            }
//        }

//        await EnsureAsync("GLU", "GLU");
//        await EnsureAsync("NA", "NA");
//    }

//    // STEP 4
//    public static async Task<long> EnsureDemoOrderAsync(IServiceProvider root, CancellationToken ct = default)
//    {
//        await using var scope = root.CreateAsyncScope();
//        var sp = scope.ServiceProvider;
//        var lab = sp.GetRequiredService<LabDbContext>();

//        var now = DateTime.UtcNow;
//        const string orderNo = "ORD-DEMO-0001";
//        const string accession = "ACC-DEMO-0001";

//        var existingReqId = await lab.LabRequests
//            .Where(r => r.OrderNo == orderNo)
//            .Select(r => r.LabRequestId)
//            .FirstOrDefaultAsync(ct);

//        if (existingReqId != 0) return existingReqId;

//        var req = new myLabRequest
//        {
//            PatientId = 1,
//            OrderNo = orderNo,
//            Priority = "Routine",
//            Status = LabRequestStatus.Requested,
//            CreatedAt = now,
//            CreatedBy = "seed"
//        };
//        lab.LabRequests.Add(req);
//        await lab.SaveChangesAsync(ct);

//        var gluId = await lab.LabTests.Where(t => t.Code == "GLU").Select(t => t.LabTestId).FirstAsync(ct);
//        var naId = await lab.LabTests.Where(t => t.Code == "NA").Select(t => t.LabTestId).FirstAsync(ct);

//        lab.LabRequestItems.AddRange(
//            new myLabRequestItem { LabRequestId = req.LabRequestId, LabTestId = gluId, CreatedAt = now, CreatedBy = "seed" },
//            new myLabRequestItem { LabRequestId = req.LabRequestId, LabTestId = naId, CreatedAt = now, CreatedBy = "seed" }
//        );
//        await lab.SaveChangesAsync(ct);

//        lab.LabSamples.Add(new myLabSample
//        {
//            LabRequestId = req.LabRequestId,
//            AccessionNumber = accession,
//            Status = LabSampleStatus.Collected,
//            CreatedAt = now,
//            CreatedBy = "seed"
//        });
//        await lab.SaveChangesAsync(ct);

//        return req.LabRequestId;
//    }
//}


////using System;
////using System.Linq;
////using System.Threading;
////using System.Threading.Tasks;
////using Microsoft.EntityFrameworkCore;
////using Microsoft.Extensions.DependencyInjection;
////using HMS.Communication.Domain.Entities;      // CommDevice
////using HMS.Communication.Domain.ValueObjects; // DeviceOptions, SerialSettings
////// Communication
////using HMS.Communication.Infrastructure.Persistence;

////// Lab
////using HMS.Module.Lab.Infrastructure.Persistence;
////using HMS.Module.Lab.Features.Lab.Models.Entities;
////using HMS.Module.Lab.Features.Lab.Models.Enums;
////using System.IO.Ports;

////namespace HMS.Api.CompositionRoot;

////public static class DevSeedHelpers
////{
////    /// <summary>
////    /// STEP 2: Ensure one demo device (ROCHE1) in the Communication DB.
////    /// Call this once at startup (safe to re-run).
////    /// </summary>
////    public static async Task SeedCommDemoAsync(IServiceProvider sp)
////    {
////        using var db = sp.GetRequiredService<CommunicationDbContext>();

////        if (!await db.Devices.AnyAsync())
////        {
////            db.Devices.Add(new CommDevice
////            {
////                DeviceCode = "ROCHE1",
////                Name = "Roche cobas e 411 (demo)",
////                IsEnabled = true,
////                Options = new DeviceOptions
////                {
////                    DeviceCode = "ROCHE1",
////                    Manufacturer = "Roche",
////                    Model = "cobas e 411",
////                    Serial = new SerialSettings
////                    {
////                        PortName = "COM3",
////                        BaudRate = 9600,
////                        DataBits = 8,
////                        Parity = Parity.None,
////                        StopBits = StopBits.One,
////                        Handshake = Handshake.None
////                    }
////                },
////                CreatedAt = DateTime.UtcNow,
////                CreatedBy = "seed"
////            });

////            await db.SaveChangesAsync();
////        }
////    }

////    /// <summary>
////    /// STEP 3: Map instrument test codes (GLU, NA) for device ROCHE1 → LIS test IDs.
////    /// </summary>
////    public static async Task SeedLabInstrumentMapAsync(IServiceProvider sp)
////    {
////        var lab = sp.GetRequiredService<LabDbContext>();
////        var comm = sp.GetRequiredService<CommunicationDbContext>();

////        var deviceId = await comm.Devices
////            .Where(d => d.DeviceCode == "ROCHE1")
////            .Select(d => d.Id) // adjust if your PK name is different
////            .FirstAsync();

////        async Task Ensure(string instCode, string lisCode)
////        {
////            var testId = await lab.LabTests
////                .Where(t => t.Code == lisCode)
////                .Select(t => t.LabTestId)
////                .FirstAsync();

////            var exists = await lab.InstrumentTestMaps.AnyAsync(m =>
////                m.DeviceId == deviceId && m.InstrumentTestCode == instCode);

////            if (!exists)
////            {
////                lab.InstrumentTestMaps.Add(new myInstrumentTestMap
////                {
////                    DeviceId = deviceId,
////                    LabTestId = testId,
////                    InstrumentTestCode = instCode,
////                    CreatedAt = DateTime.UtcNow,
////                    CreatedBy = "seed"
////                });
////                await lab.SaveChangesAsync();
////            }
////        }

////        await Ensure("GLU", "GLU");
////        await Ensure("NA", "NA");
////    }

////    /// <summary>
////    /// STEP 4: Create a demo order, items (GLU+NA) and a sample with accession ACC-DEMO-0001.
////    /// </summary>
////    public static async Task<long> EnsureDemoOrderAsync(IServiceProvider sp)
////    {
////        var lab = sp.GetRequiredService<LabDbContext>();
////        var now = DateTime.UtcNow;

////        const string orderNo = "ORD-DEMO-0001";
////        const string accession = "ACC-DEMO-0001";

////        var existingReqId = await lab.LabRequests
////            .Where(r => r.OrderNo == orderNo)
////            .Select(r => r.LabRequestId)
////            .FirstOrDefaultAsync();

////        long reqId;
////        if (existingReqId == 0)
////        {
////            var req = new myLabRequest
////            {
////                PatientId = 1, // demo
////                OrderNo = orderNo,
////                Priority = "Routine",
////                Status = LabRequestStatus.Requested,
////                CreatedAt = now,
////                CreatedBy = "seed"
////            };
////            lab.LabRequests.Add(req);
////            await lab.SaveChangesAsync();
////            reqId = req.LabRequestId;

////            var gluId = await lab.LabTests.Where(t => t.Code == "GLU").Select(t => t.LabTestId).FirstAsync();
////            var naId = await lab.LabTests.Where(t => t.Code == "NA").Select(t => t.LabTestId).FirstAsync();

////            lab.LabRequestItems.AddRange(
////                new myLabRequestItem { LabRequestId = reqId, LabTestId = gluId, CreatedAt = now, CreatedBy = "seed" },
////                new myLabRequestItem { LabRequestId = reqId, LabTestId = naId, CreatedAt = now, CreatedBy = "seed" }
////            );
////            await lab.SaveChangesAsync();

////            lab.LabSamples.Add(new myLabSample
////            {
////                LabRequestId = reqId,
////                AccessionNumber = accession,
////                Status = LabSampleStatus.Collected,
////                CreatedAt = now,
////                CreatedBy = "seed"
////            });
////            await lab.SaveChangesAsync();
////        }
////        else
////        {
////            reqId = existingReqId;
////        }

////        return reqId;
////    }
////}

// HMS.Module.Lab/Features/Lab/Infrastructure/LabDbContext.cs
using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Infrastructure.Persistence.Seed;
using HMS.SharedKernel.Base;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HMS.Module.Lab.Infrastructure.Persistence;
public sealed class LabDbContext : DbContext
{
    public LabDbContext(DbContextOptions<LabDbContext> options) : base(options) { }

    // Catalog
    public DbSet<mySpecimenType> SpecimenTypes => Set<mySpecimenType>();
    public DbSet<myReferenceRange> ReferenceRanges => Set<myReferenceRange>();
    public DbSet<myLabTest> LabTests => Set<myLabTest>();
    public DbSet<myLabPanel> LabPanels => Set<myLabPanel>();
    public DbSet<myLabPanelItem> LabPanelItems => Set<myLabPanelItem>();

    // Instruments / integration
    public DbSet<myLabInstrument> LabInstruments => Set<myLabInstrument>();
    public DbSet<myInstrumentTestMap> InstrumentTestMaps => Set<myInstrumentTestMap>();
    public DbSet<myDeviceInbox> DeviceInbox => Set<myDeviceInbox>();
    public DbSet<myDeviceOutbox> DeviceOutbox => Set<myDeviceOutbox>();

    // Orders / results
    public DbSet<myLabRequest> LabRequests => Set<myLabRequest>();
    public DbSet<myLabRequestItem> LabRequestItems => Set<myLabRequestItem>();
    public DbSet<myLabSample> LabSamples => Set<myLabSample>();
    public DbSet<myLabResult> LabResults => Set<myLabResult>();
    public DbSet<myBarcodeEvent> BarcodeEvents => Set<myBarcodeEvent>();
    public DbSet<myLabPreanalytical> LabPreanalyticals => Set<myLabPreanalytical>();
    public DbSet<myLabPatientProfile> LabPatientProfiles => Set<myLabPatientProfile>();

    //Local Lab
    public DbSet<myLabPatient> LabPatients => Set<myLabPatient>();
    public DbSet<myLabDoctor> LabDoctors => Set<myLabDoctor>();
    public DbSet<AppKv> AppKv => Set<AppKv>();

    public DbSet<SeedRun> SeedRuns => Set<SeedRun>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Soft-delete filters
        b.Entity<myLabTest>().HasQueryFilter(x => !x.IsDeleted);
        b.Entity<mySpecimenType>().HasQueryFilter(x => !x.IsDeleted);
        b.Entity<myReferenceRange>().HasQueryFilter(x => !x.IsDeleted);
        b.Entity<myLabPanel>().HasQueryFilter(x => !x.IsDeleted);
        b.Entity<myLabPanelItem>().HasQueryFilter(x => !x.IsDeleted);

        b.Entity<myLabRequest>().HasQueryFilter(x => !x.IsDeleted);
        b.Entity<myLabRequestItem>().HasQueryFilter(x => !x.IsDeleted);
        b.Entity<myLabSample>().HasQueryFilter(x => !x.IsDeleted);
        b.Entity<myLabResult>().HasQueryFilter(x => !x.IsDeleted);

        b.Entity<myInstrumentTestMap>().HasQueryFilter(x => !x.IsDeleted);
        b.Entity<myDeviceInbox>().HasQueryFilter(x => !x.IsDeleted);
        b.Entity<myDeviceOutbox>().HasQueryFilter(x => !x.IsDeleted);
        b.Entity<myLabInstrument>().HasQueryFilter(x => !x.IsDeleted);
        b.Entity<myBarcodeEvent>().HasQueryFilter(x => true); // no soft-delete   

        b.Entity<AppKv>().HasKey(x => x.Key);
        // ⛔️ No HasSequence / No UseHiLo here.
        base.OnModelCreating(b);

        // HMS.Module.Lab/Infrastructure/Persistence/LabDbContext.cs  (OnModelCreating

    }
}



//Add-Migration my_Lab -Project HMS.Module.Lab -StartupProject HMS.Api -Context LabDbContext -OutputDir Infrastructure/Persistence/Migrations
/*
 Update-Database -Project HMS.Module.Lab -StartupProject HMS.Api -Context LabDbContext
Add-Migration Init_Lab -Project HMS.Module.Lab -StartupProject HMS.Api -Context LabDbContext -OutputDir Infrastructure/Persistence/Migrations
Drop-Database -Project HMS.Module.Lab -StartupProject HMS.Api -Context LabDbContext 
Fix_FilteredUnique_LabRequestItems
migrationBuilder.Sql("""
IF NOT EXISTS (
    SELECT 1 FROM sys.sequences
    WHERE name='HmsIdSeq_Lab' AND SCHEMA_NAME(schema_id)='dbo'
)
BEGIN
  CREATE SEQUENCE [dbo].[HmsIdSeq_Lab] AS bigint
  START WITH 2025600000000 INCREMENT BY 1 CACHE 50;
END
ELSE
BEGIN
  DECLARE @inc sql_variant =
    (SELECT s.increment FROM sys.sequences s
      WHERE s.name='HmsIdSeq_Lab' AND s.schema_id=SCHEMA_ID(N'dbo'));
  IF CONVERT(bigint,@inc) <> 1
    ALTER SEQUENCE [dbo].[HmsIdSeq_Lab] INCREMENT BY 1;
END
""");



migrationBuilder.Sql("""
            IF NOT EXISTS (SELECT 1 FROM sys.sequences WHERE name='HmsIdSeq_Lab' AND SCHEMA_NAME(schema_id)='dbo')
            CREATE SEQUENCE [dbo].[HmsIdSeq_Lab] AS bigint START WITH 2025600000000 INCREMENT BY 1 NO CACHE;
            """);

// Drop tables first, then:
            migrationBuilder.Sql(@"
            IF EXISTS (SELECT 1 FROM sys.sequences WHERE name='HmsIdSeq' AND SCHEMA_NAME(schema_id)='dbo')
            DROP SEQUENCE [dbo].[HmsIdSeq];
            ");
 //comm
            migrationBuilder.Sql("""
            IF NOT EXISTS (SELECT 1 FROM sys.sequences WHERE name='HmsIdSeq' AND SCHEMA_NAME(schema_id)='dbo')
            CREATE SEQUENCE [dbo].[HmsIdSeq] AS bigint START WITH 2025900000000 INCREMENT BY 1 NO CACHE;
            """);

migrationBuilder.Sql(@"
-- Remove any old Lab_HiLo sequence to avoid confusion
IF EXISTS (SELECT 1 FROM sys.sequences WHERE name = N'Lab_HiLo' AND SCHEMA_NAME(schema_id) = N'dbo')
    DROP SEQUENCE [dbo].[Lab_HiLo];

-- Create our canonical sequence if missing
IF NOT EXISTS (SELECT 1 FROM sys.sequences WHERE name = N'HmsIdSeq_Lab' AND SCHEMA_NAME(schema_id) = N'dbo')
    CREATE SEQUENCE [dbo].[HmsIdSeq_Lab] AS bigint
        START WITH 2025600000000
        INCREMENT BY 1
        CACHE 50;  -- use cache; faster than NO CACHE
ELSE
BEGIN
    -- Ensure increment is 1 (don’t RESTART here to avoid collisions if data exists)
    DECLARE @inc sql_variant =
        (SELECT s.increment FROM sys.sequences AS s WHERE s.name = N'HmsIdSeq_Lab' AND s.schema_id = SCHEMA_ID(N'dbo'));
    IF CONVERT(bigint, @inc) <> 1
        ALTER SEQUENCE [dbo].[HmsIdSeq_Lab] INCREMENT BY 1;

    -- OPTIONAL: if you *know* DB is empty and want to force the next value:
    -- ALTER SEQUENCE [dbo].[HmsIdSeq_Lab] RESTART WITH 2025600000000;
END
");


 migrationBuilder.Sql(@"
-- Drop our sequence if present
IF EXISTS (SELECT 1 FROM sys.sequences WHERE name = N'HmsIdSeq_Lab' AND SCHEMA_NAME(schema_id) = N'dbo')
    DROP SEQUENCE [dbo].[HmsIdSeq_Lab];

-- Optionally restore the old Lab_HiLo (helps if you ever rollback)
IF NOT EXISTS (SELECT 1 FROM sys.sequences WHERE name = N'Lab_HiLo' AND SCHEMA_NAME(schema_id) = N'dbo')
    CREATE SEQUENCE [dbo].[Lab_HiLo] AS bigint
        START WITH 1000
        INCREMENT BY 10
        CACHE 50;
");

Drop-Database   -Project HMS.Module.Lab -Context LabDbContext -Force
Remove-Migration -Project HMS.Module.Lab -Context LabDbContext  # repeat until none
Add-Migration   Lab_Init -Project HMS.Module.Lab -Context LabDbContext -OutputDir Features/Lab/Infrastructure/Migrations
Update-Database -Project HMS.Module.Lab -Context LabDbContext

 */
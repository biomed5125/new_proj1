using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.Module.Lab.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Init_Lab : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
            IF NOT EXISTS (SELECT 1 FROM sys.sequences WHERE name='HmsIdSeq_Lab' AND SCHEMA_NAME(schema_id)='dbo')
            CREATE SEQUENCE [dbo].[HmsIdSeq_Lab] AS bigint START WITH 2025600000000 INCREMENT BY 1 NO CACHE;
            """);


            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "AppKv",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppKv", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "BarcodeEvents",
                columns: table => new
                {
                    BarcodeEventId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Lab"),
                    AccessionNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Event = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    At = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Who = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarcodeEvents", x => x.BarcodeEventId);
                });

            migrationBuilder.CreateTable(
                name: "DeviceInbox",
                columns: table => new
                {
                    DeviceInboxId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Lab"),
                    DeviceName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReceivedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Processed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceInbox", x => x.DeviceInboxId);
                });

            migrationBuilder.CreateTable(
                name: "DeviceOutbox",
                columns: table => new
                {
                    DeviceOutboxId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Lab"),
                    DeviceName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QueuedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Sent = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceOutbox", x => x.DeviceOutboxId);
                });

            migrationBuilder.CreateTable(
                name: "LabDoctors",
                schema: "dbo",
                columns: table => new
                {
                    LabDoctorId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Lab"),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LicenseNo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Specialty = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabDoctors", x => x.LabDoctorId);
                });

            migrationBuilder.CreateTable(
                name: "LabInstruments",
                columns: table => new
                {
                    LabInstrumentId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Lab"),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    MakeModel = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ConnectionType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Host = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Port = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabInstruments", x => x.LabInstrumentId);
                });

            migrationBuilder.CreateTable(
                name: "LabPanels",
                columns: table => new
                {
                    LabPanelId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Lab"),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabPanels", x => x.LabPanelId);
                });

            migrationBuilder.CreateTable(
                name: "LabPatients",
                schema: "dbo",
                columns: table => new
                {
                    LabPatientId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Lab"),
                    Mrn = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Sex = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabPatients", x => x.LabPatientId);
                });

            migrationBuilder.CreateTable(
                name: "SeedRuns",
                columns: table => new
                {
                    SeedRunId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppliedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeedRuns", x => x.SeedRunId);
                });

            migrationBuilder.CreateTable(
                name: "SpecimenTypes",
                columns: table => new
                {
                    SpecimenTypeId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Lab"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecimenTypes", x => x.SpecimenTypeId);
                });

            migrationBuilder.CreateTable(
                name: "LabPatientProfiles",
                schema: "dbo",
                columns: table => new
                {
                    LabPatientId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Lab"),
                    Diabetic = table.Column<bool>(type: "bit", nullable: false),
                    Thyroid = table.Column<int>(type: "int", nullable: false),
                    ChronicAnemia = table.Column<bool>(type: "bit", nullable: false),
                    Dialysis = table.Column<bool>(type: "bit", nullable: false),
                    Pacemaker = table.Column<bool>(type: "bit", nullable: false),
                    CardiacHistory = table.Column<bool>(type: "bit", nullable: false),
                    Allergy = table.Column<bool>(type: "bit", nullable: false),
                    AllergyNotes = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    FattyLiver = table.Column<bool>(type: "bit", nullable: false),
                    HighCholesterol = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabPatientProfiles", x => x.LabPatientId);
                    table.ForeignKey(
                        name: "FK_LabPatientProfiles_LabPatients_LabPatientId",
                        column: x => x.LabPatientId,
                        principalSchema: "dbo",
                        principalTable: "LabPatients",
                        principalColumn: "LabPatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabRequests",
                columns: table => new
                {
                    LabRequestId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Lab"),
                    AdmissionId = table.Column<long>(type: "bigint", nullable: true),
                    PatientId = table.Column<long>(type: "bigint", nullable: true),
                    DoctorId = table.Column<long>(type: "bigint", nullable: true),
                    LabPatientId = table.Column<long>(type: "bigint", nullable: true),
                    LabDoctorId = table.Column<long>(type: "bigint", nullable: true),
                    PatientDisplay = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: true),
                    DoctorDisplay = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: true),
                    PaidAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Priority = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OrderNo = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabRequests", x => x.LabRequestId);
                    table.ForeignKey(
                        name: "FK_LabRequests_LabDoctors_LabDoctorId",
                        column: x => x.LabDoctorId,
                        principalSchema: "dbo",
                        principalTable: "LabDoctors",
                        principalColumn: "LabDoctorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabRequests_LabPatients_LabPatientId",
                        column: x => x.LabPatientId,
                        principalSchema: "dbo",
                        principalTable: "LabPatients",
                        principalColumn: "LabPatientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LabPreanalyticals",
                columns: table => new
                {
                    LabPreanalyticalId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Lab"),
                    LabRequestId = table.Column<long>(type: "bigint", nullable: false),
                    IsDiabetic = table.Column<bool>(type: "bit", nullable: false),
                    TookAntibioticLast3Days = table.Column<bool>(type: "bit", nullable: false),
                    FastingHours = table.Column<int>(type: "int", nullable: true),
                    HasAllergy = table.Column<bool>(type: "bit", nullable: false),
                    AllergyNotes = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    ThyroidStatus = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    HasAnemia = table.Column<bool>(type: "bit", nullable: false),
                    HasFattyLiver = table.Column<bool>(type: "bit", nullable: false),
                    HasHighCholesterol = table.Column<bool>(type: "bit", nullable: false),
                    Dialysis = table.Column<bool>(type: "bit", nullable: false),
                    CardiacAttackHistory = table.Column<bool>(type: "bit", nullable: false),
                    Pacemaker = table.Column<bool>(type: "bit", nullable: false),
                    BloodPressureSys = table.Column<int>(type: "int", nullable: true),
                    BloodPressureDia = table.Column<int>(type: "int", nullable: true),
                    PulseBpm = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabPreanalyticals", x => x.LabPreanalyticalId);
                    table.ForeignKey(
                        name: "FK_LabPreanalyticals_LabRequests_LabRequestId",
                        column: x => x.LabRequestId,
                        principalTable: "LabRequests",
                        principalColumn: "LabRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabSamples",
                columns: table => new
                {
                    LabSampleId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Lab"),
                    LabRequestId = table.Column<long>(type: "bigint", nullable: false),
                    AccessionNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CollectedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CollectedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceivedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LabelPrinted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    myLabRequestLabRequestId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabSamples", x => x.LabSampleId);
                    table.ForeignKey(
                        name: "FK_LabSamples_LabRequests_LabRequestId",
                        column: x => x.LabRequestId,
                        principalTable: "LabRequests",
                        principalColumn: "LabRequestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabSamples_LabRequests_myLabRequestLabRequestId",
                        column: x => x.myLabRequestLabRequestId,
                        principalTable: "LabRequests",
                        principalColumn: "LabRequestId");
                });

            migrationBuilder.CreateTable(
                name: "InstrumentTestMap",
                columns: table => new
                {
                    InstrumentTestMapId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Lab"),
                    DeviceId = table.Column<long>(type: "bigint", nullable: false),
                    LabTestId = table.Column<long>(type: "bigint", nullable: false),
                    InstrumentTestCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    LabTestCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstrumentTestMap", x => x.InstrumentTestMapId);
                });

            migrationBuilder.CreateTable(
                name: "LabPanelItems",
                columns: table => new
                {
                    LabPanelItemId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Lab"),
                    LabPanelId = table.Column<long>(type: "bigint", nullable: false),
                    LabTestId = table.Column<long>(type: "bigint", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabPanelItems", x => x.LabPanelItemId);
                    table.ForeignKey(
                        name: "FK_LabPanelItems_LabPanels_LabPanelId",
                        column: x => x.LabPanelId,
                        principalTable: "LabPanels",
                        principalColumn: "LabPanelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabRequestItems",
                columns: table => new
                {
                    LabRequestItemId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Lab"),
                    LabRequestId = table.Column<long>(type: "bigint", nullable: false),
                    LabTestId = table.Column<long>(type: "bigint", nullable: false),
                    LabTestCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    LabTestName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    LabTestUnit = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    LabTestPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabRequestItems", x => x.LabRequestItemId);
                    table.ForeignKey(
                        name: "FK_LabRequestItems_LabRequests_LabRequestId",
                        column: x => x.LabRequestId,
                        principalTable: "LabRequests",
                        principalColumn: "LabRequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LabResults",
                columns: table => new
                {
                    LabResultId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Lab"),
                    LabRequestId = table.Column<long>(type: "bigint", nullable: false),
                    LabRequestItemId = table.Column<long>(type: "bigint", nullable: false),
                    DeviceId = table.Column<long>(type: "bigint", nullable: true),
                    AccessionNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    InstrumentTestCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    LabTestCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    LabTestName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    RefLow = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    RefHigh = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    Flag = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false, defaultValue: "Entered"),
                    RawFlag = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    ApprovedByDoctorId = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LabTestId = table.Column<long>(type: "bigint", nullable: false),
                    myLabRequestItemLabRequestItemId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabResults", x => x.LabResultId);
                    table.ForeignKey(
                        name: "FK_LabResults_LabRequestItems_myLabRequestItemLabRequestItemId",
                        column: x => x.myLabRequestItemLabRequestItemId,
                        principalTable: "LabRequestItems",
                        principalColumn: "LabRequestItemId");
                });

            migrationBuilder.CreateTable(
                name: "LabTests",
                columns: table => new
                {
                    LabTestId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Lab"),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    TatMinutes = table.Column<int>(type: "int", nullable: false),
                    RefLow = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    RefHigh = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    SpecimenTypeId = table.Column<long>(type: "bigint", nullable: false),
                    StabilityRoomHours = table.Column<int>(type: "int", nullable: true),
                    StabilityRefrigeratedHours = table.Column<int>(type: "int", nullable: true),
                    StabilityFrozenDays = table.Column<int>(type: "int", nullable: true),
                    DefaultReferenceRangeId = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTests", x => x.LabTestId);
                    table.ForeignKey(
                        name: "FK_LabTests_SpecimenTypes_SpecimenTypeId",
                        column: x => x.SpecimenTypeId,
                        principalTable: "SpecimenTypes",
                        principalColumn: "SpecimenTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReferenceRanges",
                columns: table => new
                {
                    ReferenceRangeId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Lab"),
                    RefLow = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    RefHigh = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    LabTestId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    myLabTestLabTestId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferenceRanges", x => x.ReferenceRangeId);
                    table.ForeignKey(
                        name: "FK_ReferenceRanges_LabTests_LabTestId",
                        column: x => x.LabTestId,
                        principalTable: "LabTests",
                        principalColumn: "LabTestId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReferenceRanges_LabTests_myLabTestLabTestId",
                        column: x => x.myLabTestLabTestId,
                        principalTable: "LabTests",
                        principalColumn: "LabTestId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BarcodeEvents_AccessionNumber_At",
                table: "BarcodeEvents",
                columns: new[] { "AccessionNumber", "At" });

            migrationBuilder.CreateIndex(
                name: "IX_InstrumentTestMap_DeviceId_InstrumentTestCode",
                table: "InstrumentTestMap",
                columns: new[] { "DeviceId", "InstrumentTestCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstrumentTestMap_LabTestCode",
                table: "InstrumentTestMap",
                column: "LabTestCode");

            migrationBuilder.CreateIndex(
                name: "IX_InstrumentTestMap_LabTestId",
                table: "InstrumentTestMap",
                column: "LabTestId");

            migrationBuilder.CreateIndex(
                name: "IX_LabInstruments_Name",
                table: "LabInstruments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabPanelItems_LabPanelId",
                table: "LabPanelItems",
                column: "LabPanelId");

            migrationBuilder.CreateIndex(
                name: "IX_LabPanelItems_LabTestId",
                table: "LabPanelItems",
                column: "LabTestId");

            migrationBuilder.CreateIndex(
                name: "IX_LabPanels_Code",
                table: "LabPanels",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabPreanalyticals_LabRequestId",
                table: "LabPreanalyticals",
                column: "LabRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabRequestItems_LabTestId",
                table: "LabRequestItems",
                column: "LabTestId");

            migrationBuilder.CreateIndex(
                name: "IX_LabRequestItems_Request_TestCode",
                table: "LabRequestItems",
                columns: new[] { "LabRequestId", "LabTestCode" });

            migrationBuilder.CreateIndex(
                name: "UX_LabRequestItems_Request_Test",
                table: "LabRequestItems",
                columns: new[] { "LabRequestId", "LabTestId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_LabRequests_DoctorId",
                table: "LabRequests",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_LabRequests_LabDoctorId",
                table: "LabRequests",
                column: "LabDoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_LabRequests_LabPatientId",
                table: "LabRequests",
                column: "LabPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_LabRequests_OrderNo",
                table: "LabRequests",
                column: "OrderNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabRequests_PatientId",
                table: "LabRequests",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_LabResults_AccessionNumber",
                table: "LabResults",
                column: "AccessionNumber");

            migrationBuilder.CreateIndex(
                name: "IX_LabResults_AccessionNumber_LabTestId",
                table: "LabResults",
                columns: new[] { "AccessionNumber", "LabTestId" });

            migrationBuilder.CreateIndex(
                name: "IX_LabResults_LabRequestId",
                table: "LabResults",
                column: "LabRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LabResults_LabRequestId_InstrumentTestCode",
                table: "LabResults",
                columns: new[] { "LabRequestId", "InstrumentTestCode" });

            migrationBuilder.CreateIndex(
                name: "IX_LabResults_LabRequestId_LabTestId",
                table: "LabResults",
                columns: new[] { "LabRequestId", "LabTestId" });

            migrationBuilder.CreateIndex(
                name: "IX_LabResults_LabRequestItemId",
                table: "LabResults",
                column: "LabRequestItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabResults_LabTestId",
                table: "LabResults",
                column: "LabTestId");

            migrationBuilder.CreateIndex(
                name: "IX_LabResults_myLabRequestItemLabRequestItemId",
                table: "LabResults",
                column: "myLabRequestItemLabRequestItemId");

            migrationBuilder.CreateIndex(
                name: "IX_LabSamples_AccessionNumber",
                table: "LabSamples",
                column: "AccessionNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabSamples_LabRequestId",
                table: "LabSamples",
                column: "LabRequestId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_LabSamples_myLabRequestLabRequestId",
                table: "LabSamples",
                column: "myLabRequestLabRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTests_Code",
                table: "LabTests",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabTests_DefaultReferenceRangeId",
                table: "LabTests",
                column: "DefaultReferenceRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTests_SpecimenTypeId",
                table: "LabTests",
                column: "SpecimenTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceRanges_LabTestId",
                table: "ReferenceRanges",
                column: "LabTestId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceRanges_myLabTestLabTestId",
                table: "ReferenceRanges",
                column: "myLabTestLabTestId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecimenTypes_Code",
                table: "SpecimenTypes",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SpecimenTypes_Name",
                table: "SpecimenTypes",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InstrumentTestMap_LabTests_LabTestId",
                table: "InstrumentTestMap",
                column: "LabTestId",
                principalTable: "LabTests",
                principalColumn: "LabTestId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LabPanelItems_LabTests_LabTestId",
                table: "LabPanelItems",
                column: "LabTestId",
                principalTable: "LabTests",
                principalColumn: "LabTestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabRequestItems_LabTests_LabTestId",
                table: "LabRequestItems",
                column: "LabTestId",
                principalTable: "LabTests",
                principalColumn: "LabTestId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LabResults_LabTests_LabTestId",
                table: "LabResults",
                column: "LabTestId",
                principalTable: "LabTests",
                principalColumn: "LabTestId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabTests_ReferenceRanges_DefaultReferenceRangeId",
                table: "LabTests",
                column: "DefaultReferenceRangeId",
                principalTable: "ReferenceRanges",
                principalColumn: "ReferenceRangeId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop tables first, then:
            migrationBuilder.Sql(@"
            IF EXISTS (SELECT 1 FROM sys.sequences WHERE name='HmsIdSeq_Lab' AND SCHEMA_NAME(schema_id)='dbo')
            DROP SEQUENCE [dbo].[HmsIdSeq_Lab];
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferenceRanges_LabTests_LabTestId",
                table: "ReferenceRanges");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferenceRanges_LabTests_myLabTestLabTestId",
                table: "ReferenceRanges");

            migrationBuilder.DropTable(
                name: "AppKv");

            migrationBuilder.DropTable(
                name: "BarcodeEvents");

            migrationBuilder.DropTable(
                name: "DeviceInbox");

            migrationBuilder.DropTable(
                name: "DeviceOutbox");

            migrationBuilder.DropTable(
                name: "InstrumentTestMap");

            migrationBuilder.DropTable(
                name: "LabInstruments");

            migrationBuilder.DropTable(
                name: "LabPanelItems");

            migrationBuilder.DropTable(
                name: "LabPatientProfiles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LabPreanalyticals");

            migrationBuilder.DropTable(
                name: "LabResults");

            migrationBuilder.DropTable(
                name: "LabSamples");

            migrationBuilder.DropTable(
                name: "SeedRuns");

            migrationBuilder.DropTable(
                name: "LabPanels");

            migrationBuilder.DropTable(
                name: "LabRequestItems");

            migrationBuilder.DropTable(
                name: "LabRequests");

            migrationBuilder.DropTable(
                name: "LabDoctors",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LabPatients",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LabTests");

            migrationBuilder.DropTable(
                name: "ReferenceRanges");

            migrationBuilder.DropTable(
                name: "SpecimenTypes");
        }
    }
}

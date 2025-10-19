using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.Communication.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Init_Comm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //comm
            migrationBuilder.Sql("""
            IF NOT EXISTS (SELECT 1 FROM sys.sequences WHERE name='HmsIdSeq_Comm' AND SCHEMA_NAME(schema_id)='dbo')
            CREATE SEQUENCE [dbo].[HmsIdSeq_Comm] AS bigint START WITH 2025900000000 INCREMENT BY 1 NO CACHE;
            """);

            migrationBuilder.CreateTable(
                name: "AnalyzerProfiles",
                columns: table => new
                {
                    AnalyzerProfileId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Comm"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Protocol = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    DriverClass = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    PortSettings = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DefaultMode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalyzerProfiles", x => x.AnalyzerProfileId);
                });

            migrationBuilder.CreateTable(
                name: "CommDeadLetters",
                columns: table => new
                {
                    DeadLetterResultId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Comm"),
                    DeviceId = table.Column<long>(type: "bigint", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommDeadLetters", x => x.DeadLetterResultId);
                });

            migrationBuilder.CreateTable(
                name: "CommMessageInbound",
                columns: table => new
                {
                    CommMessageInboundId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Comm"),
                    DeviceId = table.Column<long>(type: "bigint", nullable: false),
                    At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Direction = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Transport = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Ascii = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    Bytes = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    BusinessNo = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommMessageInbound", x => x.CommMessageInboundId);
                });

            migrationBuilder.CreateTable(
                name: "CommMessageOutbound",
                columns: table => new
                {
                    CommMessageOutboundId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Comm"),
                    DeviceId = table.Column<long>(type: "bigint", nullable: false),
                    At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Transport = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Sent = table.Column<bool>(type: "bit", nullable: false),
                    BusinessNo = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommMessageOutbound", x => x.CommMessageOutboundId);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    CommEventId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeviceId = table.Column<long>(type: "bigint", nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Accession = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LabTestCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstrumentCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Units = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Flag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.CommEventId);
                });

            migrationBuilder.CreateTable(
                name: "RouterRule",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Comm"),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    DeviceId = table.Column<long>(type: "bigint", nullable: true),
                    RecordType = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    TestCodeRegex = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Target = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouterRule", x => x.Id);
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
                name: "CommDevices",
                columns: table => new
                {
                    CommDeviceId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR dbo.HmsIdSeq_Comm"),
                    Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    DeviceCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    PortName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AnalyzerProfileId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommDevices", x => x.CommDeviceId);
                    table.ForeignKey(
                        name: "FK_CommDevices_AnalyzerProfiles_AnalyzerProfileId",
                        column: x => x.AnalyzerProfileId,
                        principalTable: "AnalyzerProfiles",
                        principalColumn: "AnalyzerProfileId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommDevices_AnalyzerProfileId",
                table: "CommDevices",
                column: "AnalyzerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CommDevices_DeviceCode",
                table: "CommDevices",
                column: "DeviceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RouterRule_IsEnabled_Priority",
                table: "RouterRule",
                columns: new[] { "IsEnabled", "Priority" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop tables first, then:
            migrationBuilder.Sql(@"
            IF EXISTS (SELECT 1 FROM sys.sequences WHERE name='HmsIdSeq_Comm' AND SCHEMA_NAME(schema_id)='dbo')
            DROP SEQUENCE [dbo].[HmsIdSeq_Comm];
            ");

            migrationBuilder.DropTable(
                name: "CommDeadLetters");

            migrationBuilder.DropTable(
                name: "CommDevices");

            migrationBuilder.DropTable(
                name: "CommMessageInbound");

            migrationBuilder.DropTable(
                name: "CommMessageOutbound");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "RouterRule");

            migrationBuilder.DropTable(
                name: "SeedRuns");

            migrationBuilder.DropTable(
                name: "AnalyzerProfiles");
        }
    }
}

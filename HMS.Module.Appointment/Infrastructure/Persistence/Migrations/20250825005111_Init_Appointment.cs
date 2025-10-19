using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.Module.Appointment.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Init_Appointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.sequences WHERE name='HmsIdSeq' AND SCHEMA_NAME(schema_id)='dbo')
CREATE SEQUENCE [dbo].[HmsIdSeq]
AS bigint
START WITH 2025300000000
INCREMENT BY 1
NO CACHE;
");


            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    AppointmentId = table.Column<long>(type: "bigint", nullable: false),
                    PatientId = table.Column<long>(type: "bigint", nullable: false),
                    DoctorId = table.Column<long>(type: "bigint", nullable: true),
                    ScheduledAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AppointmentNo = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.AppointmentId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppointmentNo",
                table: "Appointments",
                column: "AppointmentNo",
                unique: true,
                filter: "[AppointmentNo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorId_ScheduledAtUtc",
                table: "Appointments",
                columns: new[] { "DoctorId", "ScheduledAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId_ScheduledAtUtc",
                table: "Appointments",
                columns: new[] { "PatientId", "ScheduledAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop tables first, then:
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.sequences WHERE name='HmsIdSeq' AND SCHEMA_NAME(schema_id)='dbo')
DROP SEQUENCE [dbo].[HmsIdSeq];
");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropSequence(
                name: "HmsIdSeq");
        }
    }
}

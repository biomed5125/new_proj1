using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.Module.Patient.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Patient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.sequences WHERE name='HmsIdSeq' AND SCHEMA_NAME(schema_id)='dbo')
CREATE SEQUENCE [dbo].[HmsIdSeq]
AS bigint
START WITH 2025100000000
INCREMENT BY 1
NO CACHE;
");


            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    PatientId = table.Column<long>(type: "bigint", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Mrn = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.PatientId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Patients_FirstName_LastName_Phone_Mrn",
                table: "Patients",
                columns: new[] { "FirstName", "LastName", "Phone", "Mrn" });

            migrationBuilder.CreateIndex(
                name: "IX_Patients_Mrn",
                table: "Patients",
                column: "Mrn",
                unique: true,
                filter: "[Mrn] IS NOT NULL");
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
                name: "Patients");

            migrationBuilder.DropSequence(
                name: "HmsIdSeq");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.Module.Doctor.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Doctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            IF NOT EXISTS (SELECT 1 FROM sys.sequences WHERE name='HmsIdSeq' AND SCHEMA_NAME(schema_id)='dbo')
            CREATE SEQUENCE [dbo].[HmsIdSeq]
            AS bigint
                START WITH 2025200000000
INCREMENT BY 1
NO CACHE;
");


            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    DoctorId = table.Column<long>(type: "bigint", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Specialty = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    HireDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.DoctorId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_LastName_FirstName",
                table: "Doctors",
                columns: new[] { "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_LicenseNumber",
                table: "Doctors",
                column: "LicenseNumber",
                unique: true);
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
                name: "Doctors");

            migrationBuilder.DropSequence(
                name: "HmsIdSeq");
        }
    }
}

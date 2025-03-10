using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class educationOfficerAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EducationOfficerEndDate",
                table: "EducatorPrograms");

            migrationBuilder.DropColumn(
                name: "EducationOfficerStartDate",
                table: "EducatorPrograms");

            migrationBuilder.DropColumn(
                name: "IsProgramManager",
                table: "EducatorPrograms");

            migrationBuilder.DropColumn(
                name: "WasProgramManager",
                table: "EducatorPrograms");

            migrationBuilder.CreateTable(
                name: "EducationOfficers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProgramId = table.Column<long>(type: "bigint", nullable: true),
                    EducatorId = table.Column<long>(type: "bigint", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationOfficers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationOfficers_Educators_EducatorId",
                        column: x => x.EducatorId,
                        principalTable: "Educators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EducationOfficers_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EducationOfficers_EducatorId",
                table: "EducationOfficers",
                column: "EducatorId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationOfficers_ProgramId",
                table: "EducationOfficers",
                column: "ProgramId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EducationOfficers");

            migrationBuilder.AddColumn<DateTime>(
                name: "EducationOfficerEndDate",
                table: "EducatorPrograms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EducationOfficerStartDate",
                table: "EducatorPrograms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsProgramManager",
                table: "EducatorPrograms",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WasProgramManager",
                table: "EducatorPrograms",
                type: "boolean",
                nullable: true);
        }
    }
}

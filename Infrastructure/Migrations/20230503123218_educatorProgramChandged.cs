using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class educatorProgramChandged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "WasProgramManager",
                table: "EducatorPrograms",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EducationOfficerEndDate",
                table: "EducatorPrograms");

            migrationBuilder.DropColumn(
                name: "EducationOfficerStartDate",
                table: "EducatorPrograms");

            migrationBuilder.DropColumn(
                name: "WasProgramManager",
                table: "EducatorPrograms");
        }
    }
}

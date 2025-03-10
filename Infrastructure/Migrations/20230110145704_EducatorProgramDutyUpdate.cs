using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class EducatorProgramDutyUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DutyEndDate",
                table: "EducatorPrograms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DutyStartDate",
                table: "EducatorPrograms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DutyType",
                table: "EducatorPrograms",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DutyEndDate",
                table: "EducatorPrograms");

            migrationBuilder.DropColumn(
                name: "DutyStartDate",
                table: "EducatorPrograms");

            migrationBuilder.DropColumn(
                name: "DutyType",
                table: "EducatorPrograms");
        }
    }
}

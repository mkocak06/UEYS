using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class scientificStudyChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeginDate",
                table: "ScientificStudies");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "ScientificStudies",
                newName: "ProcessDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProcessDate",
                table: "ScientificStudies",
                newName: "EndDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "BeginDate",
                table: "ScientificStudies",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}

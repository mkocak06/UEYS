using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class secondThesisDeadline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRepeatable",
                table: "Roles");

            migrationBuilder.AddColumn<DateTime>(
                name: "SecondThesisDeadline",
                table: "EducationTrackings",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecondThesisDeadline",
                table: "EducationTrackings");

            migrationBuilder.AddColumn<bool>(
                name: "IsRepeatable",
                table: "Roles",
                type: "boolean",
                nullable: true);
        }
    }
}

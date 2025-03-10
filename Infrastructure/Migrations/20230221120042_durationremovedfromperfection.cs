using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class durationremovedfromperfection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeginDate",
                table: "StudentPerfections");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "StudentPerfections");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Perfections");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BeginDate",
                table: "StudentPerfections",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "StudentPerfections",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Perfections",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}

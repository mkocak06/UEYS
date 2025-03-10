using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class studentPerfectionsChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "StudentPerfections");

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

            migrationBuilder.AddColumn<bool>(
                name: "IsSuccessful",
                table: "StudentPerfections",
                type: "boolean",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeginDate",
                table: "StudentPerfections");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "StudentPerfections");

            migrationBuilder.DropColumn(
                name: "IsSuccessful",
                table: "StudentPerfections");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "StudentPerfections",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}

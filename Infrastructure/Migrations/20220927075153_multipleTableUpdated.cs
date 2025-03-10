using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class multipleTableUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationTrackings_Users_ProcessOwnerId",
                table: "EducationTrackings");

            migrationBuilder.DropIndex(
                name: "IX_EducationTrackings_ProcessOwnerId",
                table: "EducationTrackings");

            migrationBuilder.DropColumn(
                name: "IsCoordinator",
                table: "Educators");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "EducationTrackings");

            migrationBuilder.DropColumn(
                name: "ProcessOwnerId",
                table: "EducationTrackings");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "EducationTrackings");

            migrationBuilder.RenameColumn(
                name: "ProcessTypeId",
                table: "Reasons",
                newName: "ProcessType");

            migrationBuilder.AddColumn<bool>(
                name: "IsCoordinator",
                table: "AdvisorTheses",
                type: "boolean",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCoordinator",
                table: "AdvisorTheses");

            migrationBuilder.RenameColumn(
                name: "ProcessType",
                table: "Reasons",
                newName: "ProcessTypeId");

            migrationBuilder.AddColumn<bool>(
                name: "IsCoordinator",
                table: "Educators",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "EducationTrackings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProcessOwnerId",
                table: "EducationTrackings",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "EducationTrackings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EducationTrackings_ProcessOwnerId",
                table: "EducationTrackings",
                column: "ProcessOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationTrackings_Users_ProcessOwnerId",
                table: "EducationTrackings",
                column: "ProcessOwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

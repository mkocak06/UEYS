using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class thesisChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Theses_Educators_AdvisorId",
                table: "Theses");

            migrationBuilder.RenameColumn(
                name: "AdvisorId",
                table: "Theses",
                newName: "EducatorId");

            migrationBuilder.RenameColumn(
                name: "AdvisorAssignDate",
                table: "Theses",
                newName: "FinalExamDate");

            migrationBuilder.RenameIndex(
                name: "IX_Theses_AdvisorId",
                table: "Theses",
                newName: "IX_Theses_EducatorId");

            migrationBuilder.AddColumn<string>(
                name: "FinalExam",
                table: "Theses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AdvisorAssignDate",
                table: "AdvisorTheses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Theses_Educators_EducatorId",
                table: "Theses",
                column: "EducatorId",
                principalTable: "Educators",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Theses_Educators_EducatorId",
                table: "Theses");

            migrationBuilder.DropColumn(
                name: "FinalExam",
                table: "Theses");

            migrationBuilder.DropColumn(
                name: "AdvisorAssignDate",
                table: "AdvisorTheses");

            migrationBuilder.RenameColumn(
                name: "FinalExamDate",
                table: "Theses",
                newName: "AdvisorAssignDate");

            migrationBuilder.RenameColumn(
                name: "EducatorId",
                table: "Theses",
                newName: "AdvisorId");

            migrationBuilder.RenameIndex(
                name: "IX_Theses_EducatorId",
                table: "Theses",
                newName: "IX_Theses_AdvisorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Theses_Educators_AdvisorId",
                table: "Theses",
                column: "AdvisorId",
                principalTable: "Educators",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

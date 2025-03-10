using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class relationsAddedToStudentRotations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EducatorId",
                table: "StudentRotations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuccessful",
                table: "StudentRotations",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProgramId",
                table: "StudentRotations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EducatorId",
                table: "StudentPerfections",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Experience",
                table: "StudentPerfections",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessDate",
                table: "StudentPerfections",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProgramId",
                table: "StudentPerfections",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentRotations_EducatorId",
                table: "StudentRotations",
                column: "EducatorId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPerfections_EducatorId",
                table: "StudentPerfections",
                column: "EducatorId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPerfections_ProgramId",
                table: "StudentPerfections",
                column: "ProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentPerfections_Educators_EducatorId",
                table: "StudentPerfections",
                column: "EducatorId",
                principalTable: "Educators",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentPerfections_Programs_ProgramId",
                table: "StudentPerfections",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentRotations_Educators_EducatorId",
                table: "StudentRotations",
                column: "EducatorId",
                principalTable: "Educators",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentRotations_Programs_RotationId",
                table: "StudentRotations",
                column: "RotationId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentPerfections_Educators_EducatorId",
                table: "StudentPerfections");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentPerfections_Programs_ProgramId",
                table: "StudentPerfections");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentRotations_Educators_EducatorId",
                table: "StudentRotations");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentRotations_Programs_RotationId",
                table: "StudentRotations");

            migrationBuilder.DropIndex(
                name: "IX_StudentRotations_EducatorId",
                table: "StudentRotations");

            migrationBuilder.DropIndex(
                name: "IX_StudentPerfections_EducatorId",
                table: "StudentPerfections");

            migrationBuilder.DropIndex(
                name: "IX_StudentPerfections_ProgramId",
                table: "StudentPerfections");

            migrationBuilder.DropColumn(
                name: "EducatorId",
                table: "StudentRotations");

            migrationBuilder.DropColumn(
                name: "IsSuccessful",
                table: "StudentRotations");

            migrationBuilder.DropColumn(
                name: "ProgramId",
                table: "StudentRotations");

            migrationBuilder.DropColumn(
                name: "EducatorId",
                table: "StudentPerfections");

            migrationBuilder.DropColumn(
                name: "Experience",
                table: "StudentPerfections");

            migrationBuilder.DropColumn(
                name: "ProcessDate",
                table: "StudentPerfections");

            migrationBuilder.DropColumn(
                name: "ProgramId",
                table: "StudentPerfections");
        }
    }
}

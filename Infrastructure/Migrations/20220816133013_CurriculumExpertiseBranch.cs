using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class CurriculumExpertiseBranch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Curricula_Programs_ProgramId",
                table: "Curricula");

            migrationBuilder.DropIndex(
                name: "IX_Curricula_ProgramId",
                table: "Curricula");

            migrationBuilder.DropColumn(
                name: "ProgramId",
                table: "Curricula");

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelingDate",
                table: "ProtocolPrograms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelingProtocolNo",
                table: "ProtocolPrograms",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProtocolDate",
                table: "DependentPrograms",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<long>(
                name: "ExpertiseBranchId",
                table: "Curricula",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Curricula_ExpertiseBranchId",
                table: "Curricula",
                column: "ExpertiseBranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Curricula_ExpertiseBranches_ExpertiseBranchId",
                table: "Curricula",
                column: "ExpertiseBranchId",
                principalTable: "ExpertiseBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Curricula_ExpertiseBranches_ExpertiseBranchId",
                table: "Curricula");

            migrationBuilder.DropIndex(
                name: "IX_Curricula_ExpertiseBranchId",
                table: "Curricula");

            migrationBuilder.DropColumn(
                name: "CancelingDate",
                table: "ProtocolPrograms");

            migrationBuilder.DropColumn(
                name: "CancelingProtocolNo",
                table: "ProtocolPrograms");

            migrationBuilder.DropColumn(
                name: "ExpertiseBranchId",
                table: "Curricula");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProtocolDate",
                table: "DependentPrograms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProgramId",
                table: "Curricula",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Curricula_ProgramId",
                table: "Curricula",
                column: "ProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_Curricula_Programs_ProgramId",
                table: "Curricula",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

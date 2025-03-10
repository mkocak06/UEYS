using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class PerfectionTypeAddedToProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rotations_Curricula_CurriculumId",
                table: "Rotations");

            migrationBuilder.DropForeignKey(
                name: "FK_Rotations_ExpertiseBranches_ExpertiseBranchId",
                table: "Rotations");

            migrationBuilder.AddColumn<int>(
                name: "PerfectionType",
                table: "Properties",
                type: "integer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rotations_Curricula_CurriculumId",
                table: "Rotations",
                column: "CurriculumId",
                principalTable: "Curricula",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Rotations_ExpertiseBranches_ExpertiseBranchId",
                table: "Rotations",
                column: "ExpertiseBranchId",
                principalTable: "ExpertiseBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rotations_Curricula_CurriculumId",
                table: "Rotations");

            migrationBuilder.DropForeignKey(
                name: "FK_Rotations_ExpertiseBranches_ExpertiseBranchId",
                table: "Rotations");

            migrationBuilder.DropColumn(
                name: "PerfectionType",
                table: "Properties");

            migrationBuilder.AddForeignKey(
                name: "FK_Rotations_Curricula_CurriculumId",
                table: "Rotations",
                column: "CurriculumId",
                principalTable: "Curricula",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rotations_ExpertiseBranches_ExpertiseBranchId",
                table: "Rotations",
                column: "ExpertiseBranchId",
                principalTable: "ExpertiseBranches",
                principalColumn: "Id");
        }
    }
}

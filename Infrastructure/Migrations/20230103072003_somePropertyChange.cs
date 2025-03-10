using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class somePropertyChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducatorExpertiseBranches_Educators_EducatorId",
                table: "EducatorExpertiseBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_EducatorExpertiseBranches_ExpertiseBranches_ExpertiseBranch~",
                table: "EducatorExpertiseBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_EducatorPrograms_Educators_EducatorId",
                table: "EducatorPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_EducatorPrograms_Programs_ProgramId",
                table: "EducatorPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentExpertiseBranches_ExpertiseBranches_ExpertiseBranchId",
                table: "StudentExpertiseBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentExpertiseBranches_Students_StudentId",
                table: "StudentExpertiseBranches");

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorExpertiseBranches_Educators_EducatorId",
                table: "EducatorExpertiseBranches",
                column: "EducatorId",
                principalTable: "Educators",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorExpertiseBranches_ExpertiseBranches_ExpertiseBranch~",
                table: "EducatorExpertiseBranches",
                column: "ExpertiseBranchId",
                principalTable: "ExpertiseBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorPrograms_Educators_EducatorId",
                table: "EducatorPrograms",
                column: "EducatorId",
                principalTable: "Educators",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorPrograms_Programs_ProgramId",
                table: "EducatorPrograms",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentExpertiseBranches_ExpertiseBranches_ExpertiseBranchId",
                table: "StudentExpertiseBranches",
                column: "ExpertiseBranchId",
                principalTable: "ExpertiseBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentExpertiseBranches_Students_StudentId",
                table: "StudentExpertiseBranches",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducatorExpertiseBranches_Educators_EducatorId",
                table: "EducatorExpertiseBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_EducatorExpertiseBranches_ExpertiseBranches_ExpertiseBranch~",
                table: "EducatorExpertiseBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_EducatorPrograms_Educators_EducatorId",
                table: "EducatorPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_EducatorPrograms_Programs_ProgramId",
                table: "EducatorPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentExpertiseBranches_ExpertiseBranches_ExpertiseBranchId",
                table: "StudentExpertiseBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentExpertiseBranches_Students_StudentId",
                table: "StudentExpertiseBranches");

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorExpertiseBranches_Educators_EducatorId",
                table: "EducatorExpertiseBranches",
                column: "EducatorId",
                principalTable: "Educators",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorExpertiseBranches_ExpertiseBranches_ExpertiseBranch~",
                table: "EducatorExpertiseBranches",
                column: "ExpertiseBranchId",
                principalTable: "ExpertiseBranches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorPrograms_Educators_EducatorId",
                table: "EducatorPrograms",
                column: "EducatorId",
                principalTable: "Educators",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorPrograms_Programs_ProgramId",
                table: "EducatorPrograms",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentExpertiseBranches_ExpertiseBranches_ExpertiseBranchId",
                table: "StudentExpertiseBranches",
                column: "ExpertiseBranchId",
                principalTable: "ExpertiseBranches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentExpertiseBranches_Students_StudentId",
                table: "StudentExpertiseBranches",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StandardCurriculumRelationv1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Standards_Curricula_CurriculumId",
                table: "Standards");

            migrationBuilder.DropForeignKey(
                name: "FK_Standards_ExpertiseBranches_ExpertiseBranchId",
                table: "Standards");

            migrationBuilder.DropIndex(
                name: "IX_Standards_ExpertiseBranchId",
                table: "Standards");

            migrationBuilder.DropColumn(
                name: "ExpertiseBranchId",
                table: "Standards");

            migrationBuilder.AddForeignKey(
                name: "FK_Standards_Curricula_CurriculumId",
                table: "Standards",
                column: "CurriculumId",
                principalTable: "Curricula",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Standards_Curricula_CurriculumId",
                table: "Standards");

            migrationBuilder.AddColumn<long>(
                name: "ExpertiseBranchId",
                table: "Standards",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Standards_ExpertiseBranchId",
                table: "Standards",
                column: "ExpertiseBranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Standards_Curricula_CurriculumId",
                table: "Standards",
                column: "CurriculumId",
                principalTable: "Curricula",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Standards_ExpertiseBranches_ExpertiseBranchId",
                table: "Standards",
                column: "ExpertiseBranchId",
                principalTable: "ExpertiseBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

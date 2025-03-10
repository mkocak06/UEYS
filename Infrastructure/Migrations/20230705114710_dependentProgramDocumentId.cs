using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class dependentProgramDocumentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DependentPrograms_RelatedDependentProgram_RelatedDependentP~",
                table: "DependentPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_EducatorDependentPrograms_DependentPrograms_DependentProgra~",
                table: "EducatorDependentPrograms");

            migrationBuilder.AddColumn<long>(
                name: "DocumentId",
                table: "DependentPrograms",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DependentPrograms_RelatedDependentProgram_RelatedDependentP~",
                table: "DependentPrograms",
                column: "RelatedDependentProgramId",
                principalTable: "RelatedDependentProgram",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorDependentPrograms_DependentPrograms_DependentProgra~",
                table: "EducatorDependentPrograms",
                column: "DependentProgramId",
                principalTable: "DependentPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DependentPrograms_RelatedDependentProgram_RelatedDependentP~",
                table: "DependentPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_EducatorDependentPrograms_DependentPrograms_DependentProgra~",
                table: "EducatorDependentPrograms");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "DependentPrograms");

            migrationBuilder.AddForeignKey(
                name: "FK_DependentPrograms_RelatedDependentProgram_RelatedDependentP~",
                table: "DependentPrograms",
                column: "RelatedDependentProgramId",
                principalTable: "RelatedDependentProgram",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorDependentPrograms_DependentPrograms_DependentProgra~",
                table: "EducatorDependentPrograms",
                column: "DependentProgramId",
                principalTable: "DependentPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

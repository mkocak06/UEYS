using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class cascadeBehaviorTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvisorTheses_Educators_EducatorId",
                table: "AdvisorTheses");

            migrationBuilder.DropForeignKey(
                name: "FK_AdvisorTheses_Theses_ThesisId",
                table: "AdvisorTheses");

            migrationBuilder.AddForeignKey(
                name: "FK_AdvisorTheses_Educators_EducatorId",
                table: "AdvisorTheses",
                column: "EducatorId",
                principalTable: "Educators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdvisorTheses_Theses_ThesisId",
                table: "AdvisorTheses",
                column: "ThesisId",
                principalTable: "Theses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvisorTheses_Educators_EducatorId",
                table: "AdvisorTheses");

            migrationBuilder.DropForeignKey(
                name: "FK_AdvisorTheses_Theses_ThesisId",
                table: "AdvisorTheses");

            migrationBuilder.AddForeignKey(
                name: "FK_AdvisorTheses_Educators_EducatorId",
                table: "AdvisorTheses",
                column: "EducatorId",
                principalTable: "Educators",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AdvisorTheses_Theses_ThesisId",
                table: "AdvisorTheses",
                column: "ThesisId",
                principalTable: "Theses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

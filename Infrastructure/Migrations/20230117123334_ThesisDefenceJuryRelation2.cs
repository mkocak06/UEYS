using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class ThesisDefenceJuryRelation2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Juries_Theses_ThesisId",
                table: "Juries");

            migrationBuilder.RenameColumn(
                name: "ThesisId",
                table: "Juries",
                newName: "ThesisDefenceId");

            migrationBuilder.RenameIndex(
                name: "IX_Juries_ThesisId",
                table: "Juries",
                newName: "IX_Juries_ThesisDefenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Juries_ThesisDefences_ThesisDefenceId",
                table: "Juries",
                column: "ThesisDefenceId",
                principalTable: "ThesisDefences",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Juries_ThesisDefences_ThesisDefenceId",
                table: "Juries");

            migrationBuilder.RenameColumn(
                name: "ThesisDefenceId",
                table: "Juries",
                newName: "ThesisId");

            migrationBuilder.RenameIndex(
                name: "IX_Juries_ThesisDefenceId",
                table: "Juries",
                newName: "IX_Juries_ThesisId");

            migrationBuilder.AddForeignKey(
                name: "FK_Juries_Theses_ThesisId",
                table: "Juries",
                column: "ThesisId",
                principalTable: "Theses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

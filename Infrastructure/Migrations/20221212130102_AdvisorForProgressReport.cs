using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AdvisorForProgressReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EducatorId",
                table: "ProgressReports",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgressReports_EducatorId",
                table: "ProgressReports",
                column: "EducatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgressReports_Educators_EducatorId",
                table: "ProgressReports",
                column: "EducatorId",
                principalTable: "Educators",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProgressReports_Educators_EducatorId",
                table: "ProgressReports");

            migrationBuilder.DropIndex(
                name: "IX_ProgressReports_EducatorId",
                table: "ProgressReports");

            migrationBuilder.DropColumn(
                name: "EducatorId",
                table: "ProgressReports");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class EducationTrackingProgramRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FormerProgramId",
                table: "EducationTrackings",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EducationTrackings_FormerProgramId",
                table: "EducationTrackings",
                column: "FormerProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationTrackings_Programs_FormerProgramId",
                table: "EducationTrackings",
                column: "FormerProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationTrackings_Programs_FormerProgramId",
                table: "EducationTrackings");

            migrationBuilder.DropIndex(
                name: "IX_EducationTrackings_FormerProgramId",
                table: "EducationTrackings");

            migrationBuilder.DropColumn(
                name: "FormerProgramId",
                table: "EducationTrackings");
        }
    }
}

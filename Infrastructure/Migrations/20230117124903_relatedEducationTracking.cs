using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class relatedEducationTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RelatedEducationTrackingId",
                table: "EducationTrackings",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EducationTrackings_RelatedEducationTrackingId",
                table: "EducationTrackings",
                column: "RelatedEducationTrackingId");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationTrackings_EducationTrackings_RelatedEducationTrack~",
                table: "EducationTrackings",
                column: "RelatedEducationTrackingId",
                principalTable: "EducationTrackings",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationTrackings_EducationTrackings_RelatedEducationTrack~",
                table: "EducationTrackings");

            migrationBuilder.DropIndex(
                name: "IX_EducationTrackings_RelatedEducationTrackingId",
                table: "EducationTrackings");

            migrationBuilder.DropColumn(
                name: "RelatedEducationTrackingId",
                table: "EducationTrackings");
        }
    }
}

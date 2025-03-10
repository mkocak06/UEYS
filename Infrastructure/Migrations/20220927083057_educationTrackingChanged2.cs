using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class educationTrackingChanged2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProcessOwnerId",
                table: "EducationTrackings",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EducationTrackings_ProcessOwnerId",
                table: "EducationTrackings",
                column: "ProcessOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationTrackings_Educators_ProcessOwnerId",
                table: "EducationTrackings",
                column: "ProcessOwnerId",
                principalTable: "Educators",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationTrackings_Educators_ProcessOwnerId",
                table: "EducationTrackings");

            migrationBuilder.DropIndex(
                name: "IX_EducationTrackings_ProcessOwnerId",
                table: "EducationTrackings");

            migrationBuilder.DropColumn(
                name: "ProcessOwnerId",
                table: "EducationTrackings");
        }
    }
}

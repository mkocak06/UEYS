using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class educationTrackingChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationTrackings_Reasons_ReasonId",
                table: "EducationTrackings");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationTrackings_Reasons_ReasonId",
                table: "EducationTrackings",
                column: "ReasonId",
                principalTable: "Reasons",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationTrackings_Reasons_ReasonId",
                table: "EducationTrackings");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationTrackings_Reasons_ReasonId",
                table: "EducationTrackings",
                column: "ReasonId",
                principalTable: "Reasons",
                principalColumn: "Id");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class titlesRemovedFromStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Titles_AcademicTitleId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Titles_StaffTitleId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_AcademicTitleId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_StaffTitleId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "AcademicTitleId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "StaffTitleId",
                table: "Students");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AcademicTitleId",
                table: "Students",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StaffTitleId",
                table: "Students",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_AcademicTitleId",
                table: "Students",
                column: "AcademicTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_StaffTitleId",
                table: "Students",
                column: "StaffTitleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Titles_AcademicTitleId",
                table: "Students",
                column: "AcademicTitleId",
                principalTable: "Titles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Titles_StaffTitleId",
                table: "Students",
                column: "StaffTitleId",
                principalTable: "Titles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

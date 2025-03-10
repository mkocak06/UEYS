using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class relationsAddedToStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Titles_AcademicTitleId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Titles_StaffTitleId",
                table: "Students");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Titles_AcademicTitleId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Titles_StaffTitleId",
                table: "Students");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Titles_AcademicTitleId",
                table: "Students",
                column: "AcademicTitleId",
                principalTable: "Titles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Titles_StaffTitleId",
                table: "Students",
                column: "StaffTitleId",
                principalTable: "Titles",
                principalColumn: "Id");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class adminTitleId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AdministrativeTitleId",
                table: "Educators",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Educators_AdministrativeTitleId",
                table: "Educators",
                column: "AdministrativeTitleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Educators_Titles_AdministrativeTitleId",
                table: "Educators",
                column: "AdministrativeTitleId",
                principalTable: "Titles",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Educators_Titles_AdministrativeTitleId",
                table: "Educators");

            migrationBuilder.DropIndex(
                name: "IX_Educators_AdministrativeTitleId",
                table: "Educators");

            migrationBuilder.DropColumn(
                name: "AdministrativeTitleId",
                table: "Educators");
        }
    }
}

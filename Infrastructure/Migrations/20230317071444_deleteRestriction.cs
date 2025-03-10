using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class deleteRestriction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducatorAdministrativeTitles_Educators_EducatorId",
                table: "EducatorAdministrativeTitles");

            migrationBuilder.DropForeignKey(
                name: "FK_EducatorAdministrativeTitles_Titles_AdministrativeTitleId",
                table: "EducatorAdministrativeTitles");

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorAdministrativeTitles_Educators_EducatorId",
                table: "EducatorAdministrativeTitles",
                column: "EducatorId",
                principalTable: "Educators",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorAdministrativeTitles_Titles_AdministrativeTitleId",
                table: "EducatorAdministrativeTitles",
                column: "AdministrativeTitleId",
                principalTable: "Titles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducatorAdministrativeTitles_Educators_EducatorId",
                table: "EducatorAdministrativeTitles");

            migrationBuilder.DropForeignKey(
                name: "FK_EducatorAdministrativeTitles_Titles_AdministrativeTitleId",
                table: "EducatorAdministrativeTitles");

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorAdministrativeTitles_Educators_EducatorId",
                table: "EducatorAdministrativeTitles",
                column: "EducatorId",
                principalTable: "Educators",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorAdministrativeTitles_Titles_AdministrativeTitleId",
                table: "EducatorAdministrativeTitles",
                column: "AdministrativeTitleId",
                principalTable: "Titles",
                principalColumn: "Id");
        }
    }
}

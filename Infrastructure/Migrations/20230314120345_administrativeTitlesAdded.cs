using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class administrativeTitlesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "EducatorAdministrativeTitles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EducatorId = table.Column<long>(type: "bigint", nullable: true),
                    AdministrativeTitleId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducatorAdministrativeTitles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducatorAdministrativeTitles_Educators_EducatorId",
                        column: x => x.EducatorId,
                        principalTable: "Educators",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EducatorAdministrativeTitles_Titles_AdministrativeTitleId",
                        column: x => x.AdministrativeTitleId,
                        principalTable: "Titles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EducatorAdministrativeTitles_AdministrativeTitleId",
                table: "EducatorAdministrativeTitles",
                column: "AdministrativeTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_EducatorAdministrativeTitles_EducatorId",
                table: "EducatorAdministrativeTitles",
                column: "EducatorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EducatorAdministrativeTitles");

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
    }
}

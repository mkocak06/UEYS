using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StandardCurriculumRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Standards",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CurriculumId",
                table: "Standards",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Standards_CurriculumId",
                table: "Standards",
                column: "CurriculumId");

            migrationBuilder.AddForeignKey(
                name: "FK_Standards_Curricula_CurriculumId",
                table: "Standards",
                column: "CurriculumId",
                principalTable: "Curricula",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Standards_Curricula_CurriculumId",
                table: "Standards");

            migrationBuilder.DropIndex(
                name: "IX_Standards_CurriculumId",
                table: "Standards");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Standards");

            migrationBuilder.DropColumn(
                name: "CurriculumId",
                table: "Standards");
        }
    }
}

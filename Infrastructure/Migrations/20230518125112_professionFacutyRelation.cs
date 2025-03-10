using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class professionFacutyRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Adress",
                table: "Faculties",
                newName: "Address");

            migrationBuilder.AddColumn<long>(
                name: "ProfessionId",
                table: "Faculties",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Faculties_ProfessionId",
                table: "Faculties",
                column: "ProfessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Faculties_Professions_ProfessionId",
                table: "Faculties",
                column: "ProfessionId",
                principalTable: "Professions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Faculties_Professions_ProfessionId",
                table: "Faculties");

            migrationBuilder.DropIndex(
                name: "IX_Faculties_ProfessionId",
                table: "Faculties");

            migrationBuilder.DropColumn(
                name: "ProfessionId",
                table: "Faculties");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Faculties",
                newName: "Adress");
        }
    }
}

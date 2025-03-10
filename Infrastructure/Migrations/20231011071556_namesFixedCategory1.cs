using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class namesFixedCategory1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MinRequirements_Standarts_StandartId",
                table: "MinRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_Standarts_StandardCategories_CategoryId",
                table: "Standarts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Standarts",
                table: "Standarts");

            migrationBuilder.RenameTable(
                name: "Standarts",
                newName: "Standards");

            migrationBuilder.RenameIndex(
                name: "IX_Standarts_CategoryId",
                table: "Standards",
                newName: "IX_Standards_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Standards",
                table: "Standards",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MinRequirements_Standards_StandartId",
                table: "MinRequirements",
                column: "StandartId",
                principalTable: "Standards",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Standards_StandardCategories_CategoryId",
                table: "Standards",
                column: "CategoryId",
                principalTable: "StandardCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MinRequirements_Standards_StandartId",
                table: "MinRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_Standards_StandardCategories_CategoryId",
                table: "Standards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Standards",
                table: "Standards");

            migrationBuilder.RenameTable(
                name: "Standards",
                newName: "Standarts");

            migrationBuilder.RenameIndex(
                name: "IX_Standards_CategoryId",
                table: "Standarts",
                newName: "IX_Standarts_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Standarts",
                table: "Standarts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MinRequirements_Standarts_StandartId",
                table: "MinRequirements",
                column: "StandartId",
                principalTable: "Standarts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Standarts_StandardCategories_CategoryId",
                table: "Standarts",
                column: "CategoryId",
                principalTable: "StandardCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

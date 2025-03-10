using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class codeAddedToTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AllocatedCount",
                table: "StudentCounts",
                newName: "SecretaryAllocatedCount");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Titles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BoardAllocatedCount",
                table: "StudentCounts",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Titles");

            migrationBuilder.DropColumn(
                name: "BoardAllocatedCount",
                table: "StudentCounts");

            migrationBuilder.RenameColumn(
                name: "SecretaryAllocatedCount",
                table: "StudentCounts",
                newName: "AllocatedCount");
        }
    }
}

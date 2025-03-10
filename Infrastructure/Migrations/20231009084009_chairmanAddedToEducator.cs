using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class chairmanAddedToEducator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ForensicMedicineBoardType",
                table: "Educators",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsChairman",
                table: "Educators",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForensicMedicineBoardType",
                table: "Educators");

            migrationBuilder.DropColumn(
                name: "IsChairman",
                table: "Educators");
        }
    }
}

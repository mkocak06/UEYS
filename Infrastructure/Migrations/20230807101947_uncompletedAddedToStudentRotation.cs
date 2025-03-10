using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class uncompletedAddedToStudentRotation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUncompleted",
                table: "StudentRotations",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemainingDays",
                table: "StudentRotations",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUncompleted",
                table: "StudentRotations");

            migrationBuilder.DropColumn(
                name: "RemainingDays",
                table: "StudentRotations");
        }
    }
}

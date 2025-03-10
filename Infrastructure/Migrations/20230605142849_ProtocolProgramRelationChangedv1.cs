using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProtocolProgramRelationChangedv1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                table: "Perfections",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecialProvision",
                table: "Perfections",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "Perfections");

            migrationBuilder.DropColumn(
                name: "SpecialProvision",
                table: "Perfections");
        }
    }
}

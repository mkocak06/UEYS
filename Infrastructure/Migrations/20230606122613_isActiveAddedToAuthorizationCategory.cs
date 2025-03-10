using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class isActiveAddedToAuthorizationCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "AuthorizationCategories");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AuthorizationCategories",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AuthorizationCategories");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "AuthorizationCategories",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}

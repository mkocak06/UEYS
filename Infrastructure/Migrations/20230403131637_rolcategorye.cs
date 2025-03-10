using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class rolcategorye : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CategoryId",
                table: "Roles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Roles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RoleCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_CategoryId",
                table: "Roles",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_RoleCategories_CategoryId",
                table: "Roles",
                column: "CategoryId",
                principalTable: "RoleCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_RoleCategories_CategoryId",
                table: "Roles");

            migrationBuilder.DropTable(
                name: "RoleCategories");

            migrationBuilder.DropIndex(
                name: "IX_Roles_CategoryId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Roles");
        }
    }
}

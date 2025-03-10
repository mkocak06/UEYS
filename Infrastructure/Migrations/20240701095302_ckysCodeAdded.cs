using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ckysCodeAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CKYSCode",
                table: "Universities",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CKYSCode",
                table: "Hospitals",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CKYSCode",
                table: "ExpertiseBranches",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CKYSCode",
                table: "Universities");

            migrationBuilder.DropColumn(
                name: "CKYSCode",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "CKYSCode",
                table: "ExpertiseBranches");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class studentUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PlacementScore",
                table: "Students",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuotaType_1",
                table: "Students",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuotaType_2",
                table: "Students",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlacementScore",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "QuotaType_1",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "QuotaType_2",
                table: "Students");
        }
    }
}

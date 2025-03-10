using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class medicineInfoAddedToStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GratuatedDate",
                table: "Students",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GratuatedSchool",
                table: "Students",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MedicineRegistrationDate",
                table: "Students",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GratuatedDate",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "GratuatedSchool",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "MedicineRegistrationDate",
                table: "Students");
        }
    }
}

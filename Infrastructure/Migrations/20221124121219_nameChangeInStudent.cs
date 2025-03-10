using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class nameChangeInStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GratuatedSchool",
                table: "Students",
                newName: "GraduatedSchool");

            migrationBuilder.RenameColumn(
                name: "GratuatedDate",
                table: "Students",
                newName: "GraduatedDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GraduatedSchool",
                table: "Students",
                newName: "GratuatedSchool");

            migrationBuilder.RenameColumn(
                name: "GraduatedDate",
                table: "Students",
                newName: "GratuatedDate");
        }
    }
}

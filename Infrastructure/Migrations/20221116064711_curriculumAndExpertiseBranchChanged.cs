using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class curriculumAndExpertiseBranchChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "ExpertiseBranches");

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "Curricula",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Curricula");

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "ExpertiseBranches",
                type: "text",
                nullable: true);
        }
    }
}

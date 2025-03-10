using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExitExamNewProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ExitExams",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExamStatus",
                table: "ExitExams",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ExitExams");

            migrationBuilder.DropColumn(
                name: "ExamStatus",
                table: "ExitExams");
        }
    }
}

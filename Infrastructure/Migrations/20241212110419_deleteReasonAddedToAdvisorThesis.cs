using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class deleteReasonAddedToAdvisorThesis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeleteExplanation",
                table: "AdvisorTheses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeleteReason",
                table: "AdvisorTheses",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleteExplanation",
                table: "AdvisorTheses");

            migrationBuilder.DropColumn(
                name: "DeleteReason",
                table: "AdvisorTheses");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class thesisChanged2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EthicComitteeDecisionNo",
                table: "Theses",
                newName: "EthicCommitteeDecisionNo");

            migrationBuilder.RenameColumn(
                name: "EthicComitteeDecisionDate",
                table: "Theses",
                newName: "EthicCommitteeDecisionDate");

            migrationBuilder.RenameColumn(
                name: "EthicComitteeDecision",
                table: "Theses",
                newName: "EthicCommitteeDecision");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EthicCommitteeDecisionNo",
                table: "Theses",
                newName: "EthicComitteeDecisionNo");

            migrationBuilder.RenameColumn(
                name: "EthicCommitteeDecisionDate",
                table: "Theses",
                newName: "EthicComitteeDecisionDate");

            migrationBuilder.RenameColumn(
                name: "EthicCommitteeDecision",
                table: "Theses",
                newName: "EthicComitteeDecision");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class globalQuotaAddedToQuotaRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnnualGlobalQuota",
                table: "SubQuotaRequests");

            migrationBuilder.AddColumn<string>(
                name: "AnnualGlobalQuota",
                table: "QuotaRequests",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnnualGlobalQuota",
                table: "QuotaRequests");

            migrationBuilder.AddColumn<int>(
                name: "AnnualGlobalQuota",
                table: "SubQuotaRequests",
                type: "integer",
                nullable: true);
        }
    }
}

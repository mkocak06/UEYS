using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class userAddedToAdvisorThesis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ThesisId",
                table: "AdvisorTheses",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "EducatorId",
                table: "AdvisorTheses",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "AdvisorTheses",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "AdvisorTheses",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdvisorTheses_UserId",
                table: "AdvisorTheses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdvisorTheses_Users_UserId",
                table: "AdvisorTheses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvisorTheses_Users_UserId",
                table: "AdvisorTheses");

            migrationBuilder.DropIndex(
                name: "IX_AdvisorTheses_UserId",
                table: "AdvisorTheses");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "AdvisorTheses");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AdvisorTheses");

            migrationBuilder.AlterColumn<long>(
                name: "ThesisId",
                table: "AdvisorTheses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "EducatorId",
                table: "AdvisorTheses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}

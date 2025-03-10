using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserRoleFacultyExpertiseBranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ExpertiseBranchId",
                table: "UserRoleFaculties",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleFaculties_ExpertiseBranchId",
                table: "UserRoleFaculties",
                column: "ExpertiseBranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoleFaculties_ExpertiseBranches_ExpertiseBranchId",
                table: "UserRoleFaculties",
                column: "ExpertiseBranchId",
                principalTable: "ExpertiseBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleFaculties_ExpertiseBranches_ExpertiseBranchId",
                table: "UserRoleFaculties");

            migrationBuilder.DropIndex(
                name: "IX_UserRoleFaculties_ExpertiseBranchId",
                table: "UserRoleFaculties");

            migrationBuilder.DropColumn(
                name: "ExpertiseBranchId",
                table: "UserRoleFaculties");
        }
    }
}

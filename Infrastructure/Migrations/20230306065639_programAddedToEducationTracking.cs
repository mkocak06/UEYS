using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class programAddedToEducationTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProgramId",
                table: "EducationTrackings",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EducationTrackings_ProgramId",
                table: "EducationTrackings",
                column: "ProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationTrackings_Programs_ProgramId",
                table: "EducationTrackings",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationTrackings_Programs_ProgramId",
                table: "EducationTrackings");

            migrationBuilder.DropIndex(
                name: "IX_EducationTrackings_ProgramId",
                table: "EducationTrackings");

            migrationBuilder.DropColumn(
                name: "ProgramId",
                table: "EducationTrackings");
        }
    }
}

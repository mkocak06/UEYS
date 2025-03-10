using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExitExamEduTrackingForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EducationTrackingId",
                table: "ExitExams",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExitExams_EducationTrackingId",
                table: "ExitExams",
                column: "EducationTrackingId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExitExams_EducationTrackings_EducationTrackingId",
                table: "ExitExams",
                column: "EducationTrackingId",
                principalTable: "EducationTrackings",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExitExams_EducationTrackings_EducationTrackingId",
                table: "ExitExams");

            migrationBuilder.DropIndex(
                name: "IX_ExitExams_EducationTrackingId",
                table: "ExitExams");

            migrationBuilder.DropColumn(
                name: "EducationTrackingId",
                table: "ExitExams");
        }
    }
}

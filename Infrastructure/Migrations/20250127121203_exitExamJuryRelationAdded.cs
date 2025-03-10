using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class exitExamJuryRelationAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationTrackings_ThesisDefences_ThesisDefenceId",
                table: "EducationTrackings");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Juries_ExitExams_ExitExamId",
            //    table: "Juries");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Juries",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Juries_UserId",
                table: "Juries",
                column: "UserId");

            migrationBuilder.AddColumn<long>(
                name: "ExitExamId",
                table: "Juries",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Juries_ExitExamId",
                table: "Juries",
                column: "ExitExamId");


            migrationBuilder.AddForeignKey(
                name: "FK_EducationTrackings_ThesisDefences_ThesisDefenceId",
                table: "EducationTrackings",
                column: "ThesisDefenceId",
                principalTable: "ThesisDefences",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Juries_ExitExams_ExitExamId",
                table: "Juries",
                column: "ExitExamId",
                principalTable: "ExitExams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Juries_Users_UserId",
                table: "Juries",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationTrackings_ThesisDefences_ThesisDefenceId",
                table: "EducationTrackings");

            migrationBuilder.DropForeignKey(
                name: "FK_Juries_ExitExams_ExitExamId",
                table: "Juries");

            migrationBuilder.DropForeignKey(
                name: "FK_Juries_Users_UserId",
                table: "Juries");

            migrationBuilder.DropIndex(
                name: "IX_EducationTrackings_ThesisDefenceId",
                table: "EducationTrackings");

            migrationBuilder.DropIndex(
                name: "IX_Juries_ExitExamId",
                table: "Juries");

            migrationBuilder.DropIndex(
                name: "IX_Juries_UserId",
                table: "Juries");

            migrationBuilder.DropColumn(
               name: "ThesisDefenceId",
               table: "EducationTrackings");

            migrationBuilder.DropColumn(
                name: "ExitExamId",
                table: "Juries");

            migrationBuilder.DropColumn(
               name: "UserId",
               table: "Juries");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_EducationTrackings_ThesisDefences_ThesisDefenceId",
            //    table: "EducationTrackings",
            //    column: "ThesisDefenceId",
            //    principalTable: "ThesisDefences",
            //    principalColumn: "Id");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Juries_ExitExams_ExitExamId",
            //    table: "Juries",
            //    column: "ExitExamId",
            //    principalTable: "ExitExams",
            //    principalColumn: "Id");
        }
    }
}

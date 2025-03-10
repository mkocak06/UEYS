using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class defenceEduTrackingRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ResultDate",
                table: "ThesisDefences",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ThesisDefenceId",
                table: "EducationTrackings",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EducationTrackings_ThesisDefenceId",
                table: "EducationTrackings",
                column: "ThesisDefenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationTrackings_ThesisDefences_ThesisDefenceId",
                table: "EducationTrackings",
                column: "ThesisDefenceId",
                principalTable: "ThesisDefences",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationTrackings_ThesisDefences_ThesisDefenceId",
                table: "EducationTrackings");

            migrationBuilder.DropIndex(
                name: "IX_EducationTrackings_ThesisDefenceId",
                table: "EducationTrackings");

            migrationBuilder.DropColumn(
                name: "ResultDate",
                table: "ThesisDefences");

            migrationBuilder.DropColumn(
                name: "ThesisDefenceId",
                table: "EducationTrackings");
        }
    }
}

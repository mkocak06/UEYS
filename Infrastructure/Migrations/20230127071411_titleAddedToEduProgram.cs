using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class titleAddedToEduProgram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TitleDate",
                table: "EducatorPrograms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TitleId",
                table: "EducatorPrograms",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EducatorPrograms_TitleId",
                table: "EducatorPrograms",
                column: "TitleId");

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorPrograms_Titles_TitleId",
                table: "EducatorPrograms",
                column: "TitleId",
                principalTable: "Titles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducatorPrograms_Titles_TitleId",
                table: "EducatorPrograms");

            migrationBuilder.DropIndex(
                name: "IX_EducatorPrograms_TitleId",
                table: "EducatorPrograms");

            migrationBuilder.DropColumn(
                name: "TitleDate",
                table: "EducatorPrograms");

            migrationBuilder.DropColumn(
                name: "TitleId",
                table: "EducatorPrograms");
        }
    }
}

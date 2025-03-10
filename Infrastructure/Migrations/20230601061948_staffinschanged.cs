using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class staffinschanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Educators_Universities_UniversityId",
                table: "Educators");

            migrationBuilder.DropIndex(
                name: "IX_Educators_UniversityId",
                table: "Educators");

            migrationBuilder.DropColumn(
                name: "UniversityId",
                table: "Educators");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "EducatorStaffInstitutions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "EducatorStaffInstitutions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UniversityId",
                table: "Educators",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Educators_UniversityId",
                table: "Educators",
                column: "UniversityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Educators_Universities_UniversityId",
                table: "Educators",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "Id");
        }
    }
}

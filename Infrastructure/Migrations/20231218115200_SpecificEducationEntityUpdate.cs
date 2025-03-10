using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SpecificEducationEntityUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "StudentsSpecificEducations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "StudentsSpecificEducations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProvinceId",
                table: "SpecificEducationPlaces",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecificEducationPlaces_ProvinceId",
                table: "SpecificEducationPlaces",
                column: "ProvinceId");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificEducationPlaces_Provinces_ProvinceId",
                table: "SpecificEducationPlaces",
                column: "ProvinceId",
                principalTable: "Provinces",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecificEducationPlaces_Provinces_ProvinceId",
                table: "SpecificEducationPlaces");

            migrationBuilder.DropIndex(
                name: "IX_SpecificEducationPlaces_ProvinceId",
                table: "SpecificEducationPlaces");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "StudentsSpecificEducations");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "StudentsSpecificEducations");

            migrationBuilder.DropColumn(
                name: "ProvinceId",
                table: "SpecificEducationPlaces");
        }
    }
}

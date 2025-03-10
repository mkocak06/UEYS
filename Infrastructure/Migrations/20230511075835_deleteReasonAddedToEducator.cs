using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class deleteReasonAddedToEducator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EducatorStatus",
                table: "Educators");

            migrationBuilder.DropColumn(
                name: "IsConditionalEducator",
                table: "Educators");

            migrationBuilder.DropColumn(
                name: "TitleDate",
                table: "Educators");

            migrationBuilder.AddColumn<int>(
                name: "DeleteReason",
                table: "Educators",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeleteReasonExplanation",
                table: "Educators",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleteReason",
                table: "Educators");

            migrationBuilder.DropColumn(
                name: "DeleteReasonExplanation",
                table: "Educators");

            migrationBuilder.AddColumn<int>(
                name: "EducatorStatus",
                table: "Educators",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsConditionalEducator",
                table: "Educators",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TitleDate",
                table: "Educators",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}

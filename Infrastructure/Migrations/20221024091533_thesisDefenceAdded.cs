using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class thesisDefenceAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalExam",
                table: "Theses");

            migrationBuilder.DropColumn(
                name: "FinalExamDate",
                table: "Theses");

            migrationBuilder.DropColumn(
                name: "FinalExamResult",
                table: "Theses");

            migrationBuilder.CreateTable(
                name: "ThesisDefences",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ExamDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Result = table.Column<bool>(type: "boolean", nullable: true),
                    ThesisId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThesisDefences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThesisDefences_Theses_ThesisId",
                        column: x => x.ThesisId,
                        principalTable: "Theses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThesisDefences_ThesisId",
                table: "ThesisDefences",
                column: "ThesisId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThesisDefences");

            migrationBuilder.AddColumn<string>(
                name: "FinalExam",
                table: "Theses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FinalExamDate",
                table: "Theses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FinalExamResult",
                table: "Theses",
                type: "boolean",
                nullable: true);
        }
    }
}

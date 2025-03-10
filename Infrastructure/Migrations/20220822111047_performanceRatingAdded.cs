using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class performanceRatingAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PerformanceRatings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdditionalExplanations = table.Column<string>(type: "text", nullable: true),
                    RatingPeriod = table.Column<int>(type: "integer", nullable: true),
                    RatingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RatingResult = table.Column<int>(type: "integer", nullable: true),
                    StudentId = table.Column<long>(type: "bigint", nullable: true),
                    ProgramDirectorId = table.Column<long>(type: "bigint", nullable: true),
                    ExpertiseBranchId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformanceRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerformanceRatings_Educators_ProgramDirectorId",
                        column: x => x.ProgramDirectorId,
                        principalTable: "Educators",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PerformanceRatings_ExpertiseBranches_ExpertiseBranchId",
                        column: x => x.ExpertiseBranchId,
                        principalTable: "ExpertiseBranches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PerformanceRatings_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceRatings_ExpertiseBranchId",
                table: "PerformanceRatings",
                column: "ExpertiseBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceRatings_ProgramDirectorId",
                table: "PerformanceRatings",
                column: "ProgramDirectorId");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceRatings_StudentId",
                table: "PerformanceRatings",
                column: "StudentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PerformanceRatings");
        }
    }
}

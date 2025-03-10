using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class OpinionFormAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Juries_Educators_EducatorId",
                table: "Juries");

            migrationBuilder.DropForeignKey(
                name: "FK_Juries_Theses_ThesisId",
                table: "Juries");

            migrationBuilder.DropColumn(
                name: "AdditionalExplanations",
                table: "PerformanceRatings");

            migrationBuilder.CreateTable(
                name: "OpinionForms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ComplianceToWorkingHours_DC = table.Column<int>(type: "integer", nullable: false),
                    DutyResponsibility_DC = table.Column<int>(type: "integer", nullable: false),
                    DutyExecution_DC = table.Column<int>(type: "integer", nullable: false),
                    DutyAccomplishment_DC = table.Column<int>(type: "integer", nullable: false),
                    ComplianceToWorkingHours_MS = table.Column<int>(type: "integer", nullable: false),
                    DutyResponsibility_MS = table.Column<int>(type: "integer", nullable: false),
                    DutyExecution_MS = table.Column<int>(type: "integer", nullable: false),
                    DutyAccomplishment_MS = table.Column<int>(type: "integer", nullable: false),
                    ComplianceToWorkingHours_PE = table.Column<int>(type: "integer", nullable: false),
                    DutyResponsibility_PE = table.Column<int>(type: "integer", nullable: false),
                    DutyExecution_PE = table.Column<int>(type: "integer", nullable: false),
                    DutyAccomplishment_PE = table.Column<int>(type: "integer", nullable: false),
                    ProfessionalPracticeAbility = table.Column<int>(type: "integer", nullable: false),
                    Scientificness = table.Column<int>(type: "integer", nullable: false),
                    TeamworkAdaptation = table.Column<int>(type: "integer", nullable: false),
                    ResearchDesire = table.Column<int>(type: "integer", nullable: false),
                    ResearchExecutionAndAccomplish = table.Column<int>(type: "integer", nullable: false),
                    UsingResourcesEfficiently = table.Column<int>(type: "integer", nullable: false),
                    BroadcastingAbility = table.Column<int>(type: "integer", nullable: false),
                    AdditionalExplanation = table.Column<string>(type: "text", nullable: true),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpinionForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpinionForms_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpinionForms_StudentId",
                table: "OpinionForms",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Juries_Educators_EducatorId",
                table: "Juries",
                column: "EducatorId",
                principalTable: "Educators",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Juries_Theses_ThesisId",
                table: "Juries",
                column: "ThesisId",
                principalTable: "Theses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Juries_Educators_EducatorId",
                table: "Juries");

            migrationBuilder.DropForeignKey(
                name: "FK_Juries_Theses_ThesisId",
                table: "Juries");

            migrationBuilder.DropTable(
                name: "OpinionForms");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalExplanations",
                table: "PerformanceRatings",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Juries_Educators_EducatorId",
                table: "Juries",
                column: "EducatorId",
                principalTable: "Educators",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Juries_Theses_ThesisId",
                table: "Juries",
                column: "ThesisId",
                principalTable: "Theses",
                principalColumn: "Id");
        }
    }
}

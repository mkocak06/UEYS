using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class opinionFormAndPerformanceRatingChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OpinionForms_Educators_CreateEducatorId",
                table: "OpinionForms");

            migrationBuilder.DropForeignKey(
                name: "FK_PerformanceRatings_Educators_ProgramDirectorId",
                table: "PerformanceRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_PerformanceRatings_ExpertiseBranches_ExpertiseBranchId",
                table: "PerformanceRatings");

            migrationBuilder.DropIndex(
                name: "IX_PerformanceRatings_ExpertiseBranchId",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "ExpertiseBranchId",
                table: "PerformanceRatings");

            migrationBuilder.RenameColumn(
                name: "ProgramDirectorId",
                table: "PerformanceRatings",
                newName: "EducatorId");

            migrationBuilder.RenameIndex(
                name: "IX_PerformanceRatings_ProgramDirectorId",
                table: "PerformanceRatings",
                newName: "IX_PerformanceRatings_EducatorId");

            migrationBuilder.RenameColumn(
                name: "CreateEducatorId",
                table: "OpinionForms",
                newName: "EducatorId");

            migrationBuilder.RenameIndex(
                name: "IX_OpinionForms_CreateEducatorId",
                table: "OpinionForms",
                newName: "IX_OpinionForms_EducatorId");

            migrationBuilder.AddColumn<int>(
                name: "Altruism",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AppropriateAppealToPeople",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CommunicationObstacleRemove",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ConflictResolution",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CrisisManagement",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EmbraceLearningAndTeaching",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Empathy",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Fair",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FeedBack",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FightAddiction",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HealthProtectionVolunteer",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HealthRiskAwareness",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HumanValues",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Leadership",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LegalLiabilityAwareness",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LegalLiabilityCompletion",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LifeStyleChangeRoleModel",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Listening",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ManagementTechniquesApply",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MeetingManagement",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MotivatePeople",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NegativeNews",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SafetyProviding",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ScientificThinking",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StandUpForTeam",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeachingEffort",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorkInTeam",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorkPlaceManagement",
                table: "PerformanceRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "RatingDate",
                table: "OpinionForms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RatingResult",
                table: "OpinionForms",
                type: "integer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OpinionForms_Educators_EducatorId",
                table: "OpinionForms",
                column: "EducatorId",
                principalTable: "Educators",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PerformanceRatings_Educators_EducatorId",
                table: "PerformanceRatings",
                column: "EducatorId",
                principalTable: "Educators",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OpinionForms_Educators_EducatorId",
                table: "OpinionForms");

            migrationBuilder.DropForeignKey(
                name: "FK_PerformanceRatings_Educators_EducatorId",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "Altruism",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "AppropriateAppealToPeople",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "CommunicationObstacleRemove",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "ConflictResolution",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "CrisisManagement",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "EmbraceLearningAndTeaching",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "Empathy",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "Fair",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "FeedBack",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "FightAddiction",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "HealthProtectionVolunteer",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "HealthRiskAwareness",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "HumanValues",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "Leadership",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "LegalLiabilityAwareness",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "LegalLiabilityCompletion",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "LifeStyleChangeRoleModel",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "Listening",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "ManagementTechniquesApply",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "MeetingManagement",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "MotivatePeople",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "NegativeNews",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "SafetyProviding",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "ScientificThinking",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "StandUpForTeam",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "TeachingEffort",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "WorkInTeam",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "WorkPlaceManagement",
                table: "PerformanceRatings");

            migrationBuilder.DropColumn(
                name: "RatingDate",
                table: "OpinionForms");

            migrationBuilder.DropColumn(
                name: "RatingResult",
                table: "OpinionForms");

            migrationBuilder.RenameColumn(
                name: "EducatorId",
                table: "PerformanceRatings",
                newName: "ProgramDirectorId");

            migrationBuilder.RenameIndex(
                name: "IX_PerformanceRatings_EducatorId",
                table: "PerformanceRatings",
                newName: "IX_PerformanceRatings_ProgramDirectorId");

            migrationBuilder.RenameColumn(
                name: "EducatorId",
                table: "OpinionForms",
                newName: "CreateEducatorId");

            migrationBuilder.RenameIndex(
                name: "IX_OpinionForms_EducatorId",
                table: "OpinionForms",
                newName: "IX_OpinionForms_CreateEducatorId");

            migrationBuilder.AddColumn<long>(
                name: "ExpertiseBranchId",
                table: "PerformanceRatings",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceRatings_ExpertiseBranchId",
                table: "PerformanceRatings",
                column: "ExpertiseBranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_OpinionForms_Educators_CreateEducatorId",
                table: "OpinionForms",
                column: "CreateEducatorId",
                principalTable: "Educators",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PerformanceRatings_Educators_ProgramDirectorId",
                table: "PerformanceRatings",
                column: "ProgramDirectorId",
                principalTable: "Educators",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PerformanceRatings_ExpertiseBranches_ExpertiseBranchId",
                table: "PerformanceRatings",
                column: "ExpertiseBranchId",
                principalTable: "ExpertiseBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class opinionFormChanged1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComplianceToWorkingHours_MS",
                table: "OpinionForms");

            migrationBuilder.RenameColumn(
                name: "DutyResponsibility_PE",
                table: "OpinionForms",
                newName: "RelationsWithPatients");

            migrationBuilder.RenameColumn(
                name: "DutyResponsibility_MS",
                table: "OpinionForms",
                newName: "RelationsWithOtherStudents");

            migrationBuilder.RenameColumn(
                name: "DutyExecution_PE",
                table: "OpinionForms",
                newName: "RelationsWithOtherEmployees");

            migrationBuilder.RenameColumn(
                name: "DutyExecution_MS",
                table: "OpinionForms",
                newName: "RelationsWithEducators");

            migrationBuilder.RenameColumn(
                name: "DutyAccomplishment_PE",
                table: "OpinionForms",
                newName: "ProblemAnalysisAndSolutionAbility");

            migrationBuilder.RenameColumn(
                name: "DutyAccomplishment_MS",
                table: "OpinionForms",
                newName: "OrganizationAndCoordinationAbility");

            migrationBuilder.RenameColumn(
                name: "ComplianceToWorkingHours_PE",
                table: "OpinionForms",
                newName: "CommunicationSkills");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RelationsWithPatients",
                table: "OpinionForms",
                newName: "DutyResponsibility_PE");

            migrationBuilder.RenameColumn(
                name: "RelationsWithOtherStudents",
                table: "OpinionForms",
                newName: "DutyResponsibility_MS");

            migrationBuilder.RenameColumn(
                name: "RelationsWithOtherEmployees",
                table: "OpinionForms",
                newName: "DutyExecution_PE");

            migrationBuilder.RenameColumn(
                name: "RelationsWithEducators",
                table: "OpinionForms",
                newName: "DutyExecution_MS");

            migrationBuilder.RenameColumn(
                name: "ProblemAnalysisAndSolutionAbility",
                table: "OpinionForms",
                newName: "DutyAccomplishment_PE");

            migrationBuilder.RenameColumn(
                name: "OrganizationAndCoordinationAbility",
                table: "OpinionForms",
                newName: "DutyAccomplishment_MS");

            migrationBuilder.RenameColumn(
                name: "CommunicationSkills",
                table: "OpinionForms",
                newName: "ComplianceToWorkingHours_PE");

            migrationBuilder.AddColumn<int>(
                name: "ComplianceToWorkingHours_MS",
                table: "OpinionForms",
                type: "integer",
                nullable: true);
        }
    }
}

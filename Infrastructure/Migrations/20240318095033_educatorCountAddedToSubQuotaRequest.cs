using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class educatorCountAddedToSubQuotaRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EducatorCount",
                table: "SubQuotaRequests",
                newName: "TotalEducatorCount");

            migrationBuilder.AddColumn<int>(
                name: "AnnualGlobalQuota",
                table: "SubQuotaRequests",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssociateProfessorCount",
                table: "SubQuotaRequests",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChiefAssistantCount",
                table: "SubQuotaRequests",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentStudentCount",
                table: "SubQuotaRequests",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DoctorLecturerCount",
                table: "SubQuotaRequests",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProfessorCount",
                table: "SubQuotaRequests",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpecialistDoctorCount",
                table: "SubQuotaRequests",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentWhoLast6MonthToFinishCount",
                table: "SubQuotaRequests",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnnualGlobalQuota",
                table: "SubQuotaRequests");

            migrationBuilder.DropColumn(
                name: "AssociateProfessorCount",
                table: "SubQuotaRequests");

            migrationBuilder.DropColumn(
                name: "ChiefAssistantCount",
                table: "SubQuotaRequests");

            migrationBuilder.DropColumn(
                name: "CurrentStudentCount",
                table: "SubQuotaRequests");

            migrationBuilder.DropColumn(
                name: "DoctorLecturerCount",
                table: "SubQuotaRequests");

            migrationBuilder.DropColumn(
                name: "ProfessorCount",
                table: "SubQuotaRequests");

            migrationBuilder.DropColumn(
                name: "SpecialistDoctorCount",
                table: "SubQuotaRequests");

            migrationBuilder.DropColumn(
                name: "StudentWhoLast6MonthToFinishCount",
                table: "SubQuotaRequests");

            migrationBuilder.RenameColumn(
                name: "TotalEducatorCount",
                table: "SubQuotaRequests",
                newName: "EducatorCount");
        }
    }
}

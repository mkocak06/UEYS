using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class staffEducators : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StaffHospitalId",
                table: "Educators",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StaffParentInstitutionId",
                table: "Educators",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Educators_StaffHospitalId",
                table: "Educators",
                column: "StaffHospitalId");

            migrationBuilder.CreateIndex(
                name: "IX_Educators_StaffParentInstitutionId",
                table: "Educators",
                column: "StaffParentInstitutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Educators_Hospitals_StaffHospitalId",
                table: "Educators",
                column: "StaffHospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Educators_Institutions_StaffParentInstitutionId",
                table: "Educators",
                column: "StaffParentInstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Educators_Hospitals_StaffHospitalId",
                table: "Educators");

            migrationBuilder.DropForeignKey(
                name: "FK_Educators_Institutions_StaffParentInstitutionId",
                table: "Educators");

            migrationBuilder.DropIndex(
                name: "IX_Educators_StaffHospitalId",
                table: "Educators");

            migrationBuilder.DropIndex(
                name: "IX_Educators_StaffParentInstitutionId",
                table: "Educators");

            migrationBuilder.DropColumn(
                name: "StaffHospitalId",
                table: "Educators");

            migrationBuilder.DropColumn(
                name: "StaffParentInstitutionId",
                table: "Educators");
        }
    }
}

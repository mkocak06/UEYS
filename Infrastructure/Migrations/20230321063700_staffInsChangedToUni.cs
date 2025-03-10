using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class staffInsChangedToUni : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Educators_Institutions_StaffParentInstitutionId",
                table: "Educators");

            migrationBuilder.AddForeignKey(
                name: "FK_Educators_Universities_StaffParentInstitutionId",
                table: "Educators",
                column: "StaffParentInstitutionId",
                principalTable: "Universities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Educators_Universities_StaffParentInstitutionId",
                table: "Educators");

            migrationBuilder.AddForeignKey(
                name: "FK_Educators_Institutions_StaffParentInstitutionId",
                table: "Educators",
                column: "StaffParentInstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class staffHospitalStaffInstitution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Educators_Hospitals_HospitalId",
                table: "Educators");

            migrationBuilder.DropIndex(
                name: "IX_Educators_HospitalId",
                table: "Educators");

            migrationBuilder.DropColumn(
                name: "HospitalId",
                table: "Educators");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "HospitalId",
                table: "Educators",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Educators_HospitalId",
                table: "Educators",
                column: "HospitalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Educators_Hospitals_HospitalId",
                table: "Educators",
                column: "HospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

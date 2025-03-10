using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class ThesisDefenceHospitalForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "HospitalId",
                table: "ThesisDefences",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThesisDefences_HospitalId",
                table: "ThesisDefences",
                column: "HospitalId");

            migrationBuilder.AddForeignKey(
                name: "FK_ThesisDefences_Hospitals_HospitalId",
                table: "ThesisDefences",
                column: "HospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThesisDefences_Hospitals_HospitalId",
                table: "ThesisDefences");

            migrationBuilder.DropIndex(
                name: "IX_ThesisDefences_HospitalId",
                table: "ThesisDefences");

            migrationBuilder.DropColumn(
                name: "HospitalId",
                table: "ThesisDefences");
        }
    }
}

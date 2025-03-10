using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class hospitalAddedToEducator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hospitals_Users_ManagerId",
                table: "Hospitals");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Faculties_FacultyId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_FacultyId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Hospitals_ManagerId",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "FacultyId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Hospitals");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<long>(
                name: "FacultyId",
                table: "Users",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ManagerId",
                table: "Hospitals",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_FacultyId",
                table: "Users",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_Hospitals_ManagerId",
                table: "Hospitals",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hospitals_Users_ManagerId",
                table: "Hospitals",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Faculties_FacultyId",
                table: "Users",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

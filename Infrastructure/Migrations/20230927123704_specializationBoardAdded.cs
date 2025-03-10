using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class specializationBoardAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsForensicMedicineInstitutionEducator",
                table: "Educators",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MembershipEndDate",
                table: "Educators",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MembershipStartDate",
                table: "Educators",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsForensicMedicineInstitutionEducator",
                table: "Educators");

            migrationBuilder.DropColumn(
                name: "MembershipEndDate",
                table: "Educators");

            migrationBuilder.DropColumn(
                name: "MembershipStartDate",
                table: "Educators");
        }
    }
}

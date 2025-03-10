using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class educatorStaffAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Educators_Hospitals_StaffHospitalId",
                table: "Educators");

            migrationBuilder.DropForeignKey(
                name: "FK_Educators_Universities_StaffParentInstitutionId",
                table: "Educators");

            migrationBuilder.DropIndex(
                name: "IX_Educators_StaffHospitalId",
                table: "Educators");

            migrationBuilder.DropColumn(
                name: "StaffHospitalId",
                table: "Educators");

            migrationBuilder.RenameColumn(
                name: "StaffParentInstitutionId",
                table: "Educators",
                newName: "UniversityId");

            migrationBuilder.RenameIndex(
                name: "IX_Educators_StaffParentInstitutionId",
                table: "Educators",
                newName: "IX_Educators_UniversityId");

            migrationBuilder.CreateTable(
                name: "EducatorStaffInstitutions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EducatorId = table.Column<long>(type: "bigint", nullable: true),
                    StaffInstitutionId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducatorStaffInstitutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducatorStaffInstitutions_Educators_EducatorId",
                        column: x => x.EducatorId,
                        principalTable: "Educators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EducatorStaffInstitutions_Hospitals_StaffInstitutionId",
                        column: x => x.StaffInstitutionId,
                        principalTable: "Hospitals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducatorStaffParentInstitutions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EducatorId = table.Column<long>(type: "bigint", nullable: true),
                    StaffParentInstitutionId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducatorStaffParentInstitutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducatorStaffParentInstitutions_Educators_EducatorId",
                        column: x => x.EducatorId,
                        principalTable: "Educators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EducatorStaffParentInstitutions_Universities_StaffParentIns~",
                        column: x => x.StaffParentInstitutionId,
                        principalTable: "Universities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EducatorStaffInstitutions_EducatorId",
                table: "EducatorStaffInstitutions",
                column: "EducatorId");

            migrationBuilder.CreateIndex(
                name: "IX_EducatorStaffInstitutions_StaffInstitutionId",
                table: "EducatorStaffInstitutions",
                column: "StaffInstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_EducatorStaffParentInstitutions_EducatorId",
                table: "EducatorStaffParentInstitutions",
                column: "EducatorId");

            migrationBuilder.CreateIndex(
                name: "IX_EducatorStaffParentInstitutions_StaffParentInstitutionId",
                table: "EducatorStaffParentInstitutions",
                column: "StaffParentInstitutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Educators_Universities_UniversityId",
                table: "Educators",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Educators_Universities_UniversityId",
                table: "Educators");

            migrationBuilder.DropTable(
                name: "EducatorStaffInstitutions");

            migrationBuilder.DropTable(
                name: "EducatorStaffParentInstitutions");

            migrationBuilder.RenameColumn(
                name: "UniversityId",
                table: "Educators",
                newName: "StaffParentInstitutionId");

            migrationBuilder.RenameIndex(
                name: "IX_Educators_UniversityId",
                table: "Educators",
                newName: "IX_Educators_StaffParentInstitutionId");

            migrationBuilder.AddColumn<long>(
                name: "StaffHospitalId",
                table: "Educators",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Educators_StaffHospitalId",
                table: "Educators",
                column: "StaffHospitalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Educators_Hospitals_StaffHospitalId",
                table: "Educators",
                column: "StaffHospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Educators_Universities_StaffParentInstitutionId",
                table: "Educators",
                column: "StaffParentInstitutionId",
                principalTable: "Universities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

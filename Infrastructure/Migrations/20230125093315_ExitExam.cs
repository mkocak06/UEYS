using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class ExitExam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ExitExamId",
                table: "Juries",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExitExams",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExamDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PracticeExamNote = table.Column<int>(type: "integer", nullable: true),
                    AbilityExamNote = table.Column<int>(type: "integer", nullable: true),
                    StudentId = table.Column<long>(type: "bigint", nullable: true),
                    HospitalId = table.Column<long>(type: "bigint", nullable: true),
                    SecretaryId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExitExams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExitExams_Hospitals_HospitalId",
                        column: x => x.HospitalId,
                        principalTable: "Hospitals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ExitExams_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ExitExams_Users_SecretaryId",
                        column: x => x.SecretaryId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Juries_ExitExamId",
                table: "Juries",
                column: "ExitExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExitExams_HospitalId",
                table: "ExitExams",
                column: "HospitalId");

            migrationBuilder.CreateIndex(
                name: "IX_ExitExams_SecretaryId",
                table: "ExitExams",
                column: "SecretaryId");

            migrationBuilder.CreateIndex(
                name: "IX_ExitExams_StudentId",
                table: "ExitExams",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Juries_ExitExams_ExitExamId",
                table: "Juries",
                column: "ExitExamId",
                principalTable: "ExitExams",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Juries_ExitExams_ExitExamId",
                table: "Juries");

            migrationBuilder.DropTable(
                name: "ExitExams");

            migrationBuilder.DropIndex(
                name: "IX_Juries_ExitExamId",
                table: "Juries");

            migrationBuilder.DropColumn(
                name: "ExitExamId",
                table: "Juries");
        }
    }
}

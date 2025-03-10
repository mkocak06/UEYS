using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class specificeducation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpecificEducation",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CurriculumId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificEducation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpecificEducation_Curricula_CurriculumId",
                        column: x => x.CurriculumId,
                        principalTable: "Curricula",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SpecificEducationPlace",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificEducationPlace", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentsSpecificEducation",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SpesificEducationId = table.Column<long>(type: "bigint", nullable: true),
                    SpecificEducationId = table.Column<long>(type: "bigint", nullable: true),
                    StudentId = table.Column<long>(type: "bigint", nullable: true),
                    SpecificEducationPlaceId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentsSpecificEducation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentsSpecificEducation_SpecificEducationPlace_SpecificEd~",
                        column: x => x.SpecificEducationPlaceId,
                        principalTable: "SpecificEducationPlace",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentsSpecificEducation_SpecificEducation_SpecificEducati~",
                        column: x => x.SpecificEducationId,
                        principalTable: "SpecificEducation",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentsSpecificEducation_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpecificEducation_CurriculumId",
                table: "SpecificEducation",
                column: "CurriculumId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentsSpecificEducation_SpecificEducationId",
                table: "StudentsSpecificEducation",
                column: "SpecificEducationId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentsSpecificEducation_SpecificEducationPlaceId",
                table: "StudentsSpecificEducation",
                column: "SpecificEducationPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentsSpecificEducation_StudentId",
                table: "StudentsSpecificEducation",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentsSpecificEducation");

            migrationBuilder.DropTable(
                name: "SpecificEducationPlace");

            migrationBuilder.DropTable(
                name: "SpecificEducation");
        }
    }
}

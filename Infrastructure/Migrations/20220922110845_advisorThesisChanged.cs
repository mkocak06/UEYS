using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class advisorThesisChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EducatorTheses");

            migrationBuilder.CreateTable(
                name: "AdvisorTheses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EducatorId = table.Column<long>(type: "bigint", nullable: false),
                    ThesisId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvisorTheses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdvisorTheses_Educators_EducatorId",
                        column: x => x.EducatorId,
                        principalTable: "Educators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AdvisorTheses_Theses_ThesisId",
                        column: x => x.ThesisId,
                        principalTable: "Theses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdvisorTheses_EducatorId",
                table: "AdvisorTheses",
                column: "EducatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvisorTheses_ThesisId",
                table: "AdvisorTheses",
                column: "ThesisId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvisorTheses");

            migrationBuilder.CreateTable(
                name: "EducatorTheses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EducatorId = table.Column<long>(type: "bigint", nullable: false),
                    ThesisId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducatorTheses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducatorTheses_Educators_EducatorId",
                        column: x => x.EducatorId,
                        principalTable: "Educators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EducatorTheses_Theses_ThesisId",
                        column: x => x.ThesisId,
                        principalTable: "Theses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EducatorTheses_EducatorId",
                table: "EducatorTheses",
                column: "EducatorId");

            migrationBuilder.CreateIndex(
                name: "IX_EducatorTheses_ThesisId",
                table: "EducatorTheses",
                column: "ThesisId");
        }
    }
}

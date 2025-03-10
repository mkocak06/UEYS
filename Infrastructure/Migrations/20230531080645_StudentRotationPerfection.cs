using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StudentRotationPerfection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentRotationPerfections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProcessDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsSuccessful = table.Column<bool>(type: "boolean", nullable: true),
                    EducatorId = table.Column<long>(type: "bigint", nullable: true),
                    PerfectionId = table.Column<long>(type: "bigint", nullable: true),
                    StudentRotationId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentRotationPerfections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentRotationPerfections_Educators_EducatorId",
                        column: x => x.EducatorId,
                        principalTable: "Educators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_StudentRotationPerfections_Perfections_PerfectionId",
                        column: x => x.PerfectionId,
                        principalTable: "Perfections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_StudentRotationPerfections_StudentRotations_StudentRotation~",
                        column: x => x.StudentRotationId,
                        principalTable: "StudentRotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentRotationPerfections_EducatorId",
                table: "StudentRotationPerfections",
                column: "EducatorId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentRotationPerfections_PerfectionId",
                table: "StudentRotationPerfections",
                column: "PerfectionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentRotationPerfections_StudentRotationId",
                table: "StudentRotationPerfections",
                column: "StudentRotationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentRotationPerfections");
        }
    }
}

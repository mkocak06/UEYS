using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class curriculumRotationAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rotations_Curricula_CurriculumId",
                table: "Rotations");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentPerfections_StudentRotations_StudentRotationId",
                table: "StudentPerfections");

            migrationBuilder.DropIndex(
                name: "IX_StudentPerfections_StudentRotationId",
                table: "StudentPerfections");

            migrationBuilder.DropIndex(
                name: "IX_Rotations_CurriculumId",
                table: "Rotations");

            migrationBuilder.DropColumn(
                name: "StudentRotationId",
                table: "StudentPerfections");

            migrationBuilder.DropColumn(
                name: "CurriculumId",
                table: "Rotations");

            migrationBuilder.CreateTable(
                name: "CurriculumRotations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CurriculumId = table.Column<long>(type: "bigint", nullable: true),
                    RotationId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurriculumRotations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurriculumRotations_Curricula_CurriculumId",
                        column: x => x.CurriculumId,
                        principalTable: "Curricula",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CurriculumRotations_Rotations_RotationId",
                        column: x => x.RotationId,
                        principalTable: "Rotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurriculumRotations_CurriculumId",
                table: "CurriculumRotations",
                column: "CurriculumId");

            migrationBuilder.CreateIndex(
                name: "IX_CurriculumRotations_RotationId",
                table: "CurriculumRotations",
                column: "RotationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurriculumRotations");

            migrationBuilder.AddColumn<long>(
                name: "StudentRotationId",
                table: "StudentPerfections",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CurriculumId",
                table: "Rotations",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentPerfections_StudentRotationId",
                table: "StudentPerfections",
                column: "StudentRotationId");

            migrationBuilder.CreateIndex(
                name: "IX_Rotations_CurriculumId",
                table: "Rotations",
                column: "CurriculumId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rotations_Curricula_CurriculumId",
                table: "Rotations",
                column: "CurriculumId",
                principalTable: "Curricula",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentPerfections_StudentRotations_StudentRotationId",
                table: "StudentPerfections",
                column: "StudentRotationId",
                principalTable: "StudentRotations",
                principalColumn: "Id");
        }
    }
}

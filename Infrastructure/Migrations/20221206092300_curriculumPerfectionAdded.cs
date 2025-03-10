using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class curriculumPerfectionAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Perfections_Curricula_CurriculumId",
                table: "Perfections");

            migrationBuilder.DropForeignKey(
                name: "FK_Perfections_Rotations_RotationId",
                table: "Perfections");

            migrationBuilder.DropIndex(
                name: "IX_Perfections_CurriculumId",
                table: "Perfections");

            migrationBuilder.DropIndex(
                name: "IX_Perfections_RotationId",
                table: "Perfections");

            migrationBuilder.DropColumn(
                name: "CurriculumId",
                table: "Perfections");

            migrationBuilder.DropColumn(
                name: "RotationId",
                table: "Perfections");

            migrationBuilder.CreateTable(
                name: "CurriculumPerfections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CurriculumId = table.Column<long>(type: "bigint", nullable: true),
                    PerfectionId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurriculumPerfections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurriculumPerfections_Curricula_CurriculumId",
                        column: x => x.CurriculumId,
                        principalTable: "Curricula",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CurriculumPerfections_Perfections_PerfectionId",
                        column: x => x.PerfectionId,
                        principalTable: "Perfections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurriculumPerfections_CurriculumId",
                table: "CurriculumPerfections",
                column: "CurriculumId");

            migrationBuilder.CreateIndex(
                name: "IX_CurriculumPerfections_PerfectionId",
                table: "CurriculumPerfections",
                column: "PerfectionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurriculumPerfections");

            migrationBuilder.AddColumn<long>(
                name: "CurriculumId",
                table: "Perfections",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RotationId",
                table: "Perfections",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Perfections_CurriculumId",
                table: "Perfections",
                column: "CurriculumId");

            migrationBuilder.CreateIndex(
                name: "IX_Perfections_RotationId",
                table: "Perfections",
                column: "RotationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Perfections_Curricula_CurriculumId",
                table: "Perfections",
                column: "CurriculumId",
                principalTable: "Curricula",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Perfections_Rotations_RotationId",
                table: "Perfections",
                column: "RotationId",
                principalTable: "Rotations",
                principalColumn: "Id");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class ComplementProgramAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PerformanceRatings_Educators_ProgramDirectorId",
                table: "PerformanceRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_PerformanceRatings_ExpertiseBranches_ExpertiseBranchId",
                table: "PerformanceRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_PerformanceRatings_Students_StudentId",
                table: "PerformanceRatings");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ProtocolPrograms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ComplementProgramId",
                table: "DependentPrograms",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ComplementPrograms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProtocolNo = table.Column<string>(type: "text", nullable: true),
                    ProtocolDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancelingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancelingProtocolNo = table.Column<string>(type: "text", nullable: true),
                    ParentProgramId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplementPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComplementPrograms_Programs_ParentProgramId",
                        column: x => x.ParentProgramId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DependentPrograms_ComplementProgramId",
                table: "DependentPrograms",
                column: "ComplementProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplementPrograms_ParentProgramId",
                table: "ComplementPrograms",
                column: "ParentProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_DependentPrograms_ComplementPrograms_ComplementProgramId",
                table: "DependentPrograms",
                column: "ComplementProgramId",
                principalTable: "ComplementPrograms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PerformanceRatings_Educators_ProgramDirectorId",
                table: "PerformanceRatings",
                column: "ProgramDirectorId",
                principalTable: "Educators",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PerformanceRatings_ExpertiseBranches_ExpertiseBranchId",
                table: "PerformanceRatings",
                column: "ExpertiseBranchId",
                principalTable: "ExpertiseBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PerformanceRatings_Students_StudentId",
                table: "PerformanceRatings",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DependentPrograms_ComplementPrograms_ComplementProgramId",
                table: "DependentPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_PerformanceRatings_Educators_ProgramDirectorId",
                table: "PerformanceRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_PerformanceRatings_ExpertiseBranches_ExpertiseBranchId",
                table: "PerformanceRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_PerformanceRatings_Students_StudentId",
                table: "PerformanceRatings");

            migrationBuilder.DropTable(
                name: "ComplementPrograms");

            migrationBuilder.DropIndex(
                name: "IX_DependentPrograms_ComplementProgramId",
                table: "DependentPrograms");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ProtocolPrograms");

            migrationBuilder.DropColumn(
                name: "ComplementProgramId",
                table: "DependentPrograms");

            migrationBuilder.AddForeignKey(
                name: "FK_PerformanceRatings_Educators_ProgramDirectorId",
                table: "PerformanceRatings",
                column: "ProgramDirectorId",
                principalTable: "Educators",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PerformanceRatings_ExpertiseBranches_ExpertiseBranchId",
                table: "PerformanceRatings",
                column: "ExpertiseBranchId",
                principalTable: "ExpertiseBranches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PerformanceRatings_Students_StudentId",
                table: "PerformanceRatings",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id");
        }
    }
}

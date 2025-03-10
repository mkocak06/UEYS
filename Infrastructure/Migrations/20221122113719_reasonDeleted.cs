using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class reasonDeleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationTrackings_Reasons_ReasonId",
                table: "EducationTrackings");

            migrationBuilder.DropTable(
                name: "Reasons");

            migrationBuilder.DropIndex(
                name: "IX_EducationTrackings_ReasonId",
                table: "EducationTrackings");

            migrationBuilder.DropColumn(
                name: "ReasonId",
                table: "EducationTrackings");

            migrationBuilder.AddColumn<int>(
                name: "ReasonType",
                table: "EducationTrackings",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReasonType",
                table: "EducationTrackings");

            migrationBuilder.AddColumn<long>(
                name: "ReasonId",
                table: "EducationTrackings",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Reasons",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ColorCode = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ProcessType = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reasons", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EducationTrackings_ReasonId",
                table: "EducationTrackings",
                column: "ReasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationTrackings_Reasons_ReasonId",
                table: "EducationTrackings",
                column: "ReasonId",
                principalTable: "Reasons",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

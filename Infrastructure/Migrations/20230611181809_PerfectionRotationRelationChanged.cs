using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PerfectionRotationRelationChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CurriculumRotationId",
                table: "Perfections",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Perfections_CurriculumRotationId",
                table: "Perfections",
                column: "CurriculumRotationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Perfections_CurriculumRotations_CurriculumRotationId",
                table: "Perfections",
                column: "CurriculumRotationId",
                principalTable: "CurriculumRotations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Perfections_CurriculumRotations_CurriculumRotationId",
                table: "Perfections");

            migrationBuilder.DropIndex(
                name: "IX_Perfections_CurriculumRotationId",
                table: "Perfections");

            migrationBuilder.DropColumn(
                name: "CurriculumRotationId",
                table: "Perfections");
        }
    }
}

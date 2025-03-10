using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RotationPerfectionRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RotationId",
                table: "Perfections",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Perfections_RotationId",
                table: "Perfections",
                column: "RotationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Perfections_Rotations_RotationId",
                table: "Perfections",
                column: "RotationId",
                principalTable: "Rotations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Perfections_Rotations_RotationId",
                table: "Perfections");

            migrationBuilder.DropIndex(
                name: "IX_Perfections_RotationId",
                table: "Perfections");

            migrationBuilder.DropColumn(
                name: "RotationId",
                table: "Perfections");
        }
    }
}

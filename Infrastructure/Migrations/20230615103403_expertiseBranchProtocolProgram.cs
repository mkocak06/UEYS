using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class expertiseBranchProtocolProgram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Perfections_CurriculumRotations_CurriculumRotationId",
                table: "Perfections");

            migrationBuilder.AddColumn<int>(
                name: "ProtocolProgramCount",
                table: "ExpertiseBranches",
                type: "integer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Perfections_CurriculumRotations_CurriculumRotationId",
                table: "Perfections",
                column: "CurriculumRotationId",
                principalTable: "CurriculumRotations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Perfections_CurriculumRotations_CurriculumRotationId",
                table: "Perfections");

            migrationBuilder.DropColumn(
                name: "ProtocolProgramCount",
                table: "ExpertiseBranches");

            migrationBuilder.AddForeignKey(
                name: "FK_Perfections_CurriculumRotations_CurriculumRotationId",
                table: "Perfections",
                column: "CurriculumRotationId",
                principalTable: "CurriculumRotations",
                principalColumn: "Id");
        }
    }
}

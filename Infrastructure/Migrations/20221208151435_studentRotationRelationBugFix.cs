using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class studentRotationRelationBugFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentRotations_Programs_RotationId",
                table: "StudentRotations");

            migrationBuilder.CreateIndex(
                name: "IX_StudentRotations_ProgramId",
                table: "StudentRotations",
                column: "ProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentRotations_Programs_ProgramId",
                table: "StudentRotations",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentRotations_Programs_ProgramId",
                table: "StudentRotations");

            migrationBuilder.DropIndex(
                name: "IX_StudentRotations_ProgramId",
                table: "StudentRotations");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentRotations_Programs_RotationId",
                table: "StudentRotations",
                column: "RotationId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

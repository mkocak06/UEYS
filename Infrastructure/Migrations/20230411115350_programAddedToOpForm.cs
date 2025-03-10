using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class programAddedToOpForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProgramId",
                table: "OpinionForms",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpinionForms_ProgramId",
                table: "OpinionForms",
                column: "ProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_OpinionForms_Programs_ProgramId",
                table: "OpinionForms",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OpinionForms_Programs_ProgramId",
                table: "OpinionForms");

            migrationBuilder.DropIndex(
                name: "IX_OpinionForms_ProgramId",
                table: "OpinionForms");

            migrationBuilder.DropColumn(
                name: "ProgramId",
                table: "OpinionForms");
        }
    }
}

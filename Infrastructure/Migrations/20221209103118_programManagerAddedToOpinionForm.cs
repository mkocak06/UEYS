using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class programManagerAddedToOpinionForm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProgramManagerId",
                table: "OpinionForms",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpinionForms_ProgramManagerId",
                table: "OpinionForms",
                column: "ProgramManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_OpinionForms_Educators_ProgramManagerId",
                table: "OpinionForms",
                column: "ProgramManagerId",
                principalTable: "Educators",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OpinionForms_Educators_ProgramManagerId",
                table: "OpinionForms");

            migrationBuilder.DropIndex(
                name: "IX_OpinionForms_ProgramManagerId",
                table: "OpinionForms");

            migrationBuilder.DropColumn(
                name: "ProgramManagerId",
                table: "OpinionForms");
        }
    }
}

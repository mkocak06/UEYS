using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class SecretaryAddedToOpForm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SecretaryId",
                table: "OpinionForms",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpinionForms_SecretaryId",
                table: "OpinionForms",
                column: "SecretaryId");

            migrationBuilder.AddForeignKey(
                name: "FK_OpinionForms_Users_SecretaryId",
                table: "OpinionForms",
                column: "SecretaryId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OpinionForms_Users_SecretaryId",
                table: "OpinionForms");

            migrationBuilder.DropIndex(
                name: "IX_OpinionForms_SecretaryId",
                table: "OpinionForms");

            migrationBuilder.DropColumn(
                name: "SecretaryId",
                table: "OpinionForms");
        }
    }
}

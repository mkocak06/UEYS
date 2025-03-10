using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class opinionFormChanged2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "StudentId",
                table: "OpinionForms",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "CreateEducatorId",
                table: "OpinionForms",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpinionForms_CreateEducatorId",
                table: "OpinionForms",
                column: "CreateEducatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_OpinionForms_Educators_CreateEducatorId",
                table: "OpinionForms",
                column: "CreateEducatorId",
                principalTable: "Educators",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OpinionForms_Educators_CreateEducatorId",
                table: "OpinionForms");

            migrationBuilder.DropIndex(
                name: "IX_OpinionForms_CreateEducatorId",
                table: "OpinionForms");

            migrationBuilder.DropColumn(
                name: "CreateEducatorId",
                table: "OpinionForms");

            migrationBuilder.AlterColumn<long>(
                name: "StudentId",
                table: "OpinionForms",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}

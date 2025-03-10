using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class educatorProgramAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducatorPrograms_Educators_EducatorId",
                table: "EducatorPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_EducatorPrograms_Programs_ProgramId",
                table: "EducatorPrograms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EducatorPrograms",
                table: "EducatorPrograms");

            migrationBuilder.AlterColumn<long>(
                name: "ProgramId",
                table: "EducatorPrograms",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "EducatorId",
                table: "EducatorPrograms",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "EducatorPrograms",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EducatorPrograms",
                table: "EducatorPrograms",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EducatorPrograms_EducatorId",
                table: "EducatorPrograms",
                column: "EducatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorPrograms_Educators_EducatorId",
                table: "EducatorPrograms",
                column: "EducatorId",
                principalTable: "Educators",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorPrograms_Programs_ProgramId",
                table: "EducatorPrograms",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducatorPrograms_Educators_EducatorId",
                table: "EducatorPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_EducatorPrograms_Programs_ProgramId",
                table: "EducatorPrograms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EducatorPrograms",
                table: "EducatorPrograms");

            migrationBuilder.DropIndex(
                name: "IX_EducatorPrograms_EducatorId",
                table: "EducatorPrograms");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "EducatorPrograms");

            migrationBuilder.AlterColumn<long>(
                name: "ProgramId",
                table: "EducatorPrograms",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "EducatorId",
                table: "EducatorPrograms",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EducatorPrograms",
                table: "EducatorPrograms",
                columns: new[] { "EducatorId", "ProgramId" });

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorPrograms_Educators_EducatorId",
                table: "EducatorPrograms",
                column: "EducatorId",
                principalTable: "Educators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorPrograms_Programs_ProgramId",
                table: "EducatorPrograms",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

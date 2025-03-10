using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class idPerfectionProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PerfectionProperties",
                table: "PerfectionProperties");

            migrationBuilder.AlterColumn<long>(
                name: "PerfectionId",
                table: "PerfectionProperties",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "PropertyId",
                table: "PerfectionProperties",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "PerfectionProperties",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PerfectionProperties",
                table: "PerfectionProperties",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PerfectionProperties_PropertyId",
                table: "PerfectionProperties",
                column: "PropertyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PerfectionProperties",
                table: "PerfectionProperties");

            migrationBuilder.DropIndex(
                name: "IX_PerfectionProperties_PropertyId",
                table: "PerfectionProperties");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PerfectionProperties");

            migrationBuilder.AlterColumn<long>(
                name: "PropertyId",
                table: "PerfectionProperties",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "PerfectionId",
                table: "PerfectionProperties",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PerfectionProperties",
                table: "PerfectionProperties",
                columns: new[] { "PropertyId", "PerfectionId" });
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class ComplementProgramRemoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DependentPrograms_ComplementPrograms_ComplementProgramId",
                table: "DependentPrograms");

            migrationBuilder.DropTable(
                name: "ComplementPrograms");

            migrationBuilder.DropIndex(
                name: "IX_DependentPrograms_ComplementProgramId",
                table: "DependentPrograms");

            migrationBuilder.DropColumn(
                name: "ComplementProgramId",
                table: "DependentPrograms");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ComplementProgramId",
                table: "DependentPrograms",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ComplementPrograms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParentProgramId = table.Column<long>(type: "bigint", nullable: true),
                    CancelingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancelingProtocolNo = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ProtocolDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProtocolNo = table.Column<string>(type: "text", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplementPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComplementPrograms_Programs_ParentProgramId",
                        column: x => x.ParentProgramId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DependentPrograms_ComplementProgramId",
                table: "DependentPrograms",
                column: "ComplementProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplementPrograms_ParentProgramId",
                table: "ComplementPrograms",
                column: "ParentProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_DependentPrograms_ComplementPrograms_ComplementProgramId",
                table: "DependentPrograms",
                column: "ComplementProgramId",
                principalTable: "ComplementPrograms",
                principalColumn: "Id");
        }
    }
}

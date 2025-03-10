using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProtocolProgramRelationChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DependentPrograms_ProtocolPrograms_ProtocolProgramId",
                table: "DependentPrograms");

            migrationBuilder.DropColumn(
                name: "CancelingDate",
                table: "ProtocolPrograms");

            migrationBuilder.DropColumn(
                name: "CancelingProtocolNo",
                table: "ProtocolPrograms");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "ProtocolPrograms");

            migrationBuilder.DropColumn(
                name: "ProtocolDate",
                table: "ProtocolPrograms");

            migrationBuilder.RenameColumn(
                name: "ProtocolProgramId",
                table: "DependentPrograms",
                newName: "RelatedDependentProgramId");

            migrationBuilder.RenameIndex(
                name: "IX_DependentPrograms_ProtocolProgramId",
                table: "DependentPrograms",
                newName: "IX_DependentPrograms_RelatedDependentProgramId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelingDate",
                table: "DependentPrograms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelingProtocolNo",
                table: "DependentPrograms",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RelatedDependentProgram",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ProtocolProgramId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatedDependentProgram", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelatedDependentProgram_ProtocolPrograms_ProtocolProgramId",
                        column: x => x.ProtocolProgramId,
                        principalTable: "ProtocolPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RelatedDependentProgram_ProtocolProgramId",
                table: "RelatedDependentProgram",
                column: "ProtocolProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_DependentPrograms_RelatedDependentProgram_RelatedDependentP~",
                table: "DependentPrograms",
                column: "RelatedDependentProgramId",
                principalTable: "RelatedDependentProgram",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DependentPrograms_RelatedDependentProgram_RelatedDependentP~",
                table: "DependentPrograms");

            migrationBuilder.DropTable(
                name: "RelatedDependentProgram");

            migrationBuilder.DropColumn(
                name: "CancelingDate",
                table: "DependentPrograms");

            migrationBuilder.DropColumn(
                name: "CancelingProtocolNo",
                table: "DependentPrograms");

            migrationBuilder.RenameColumn(
                name: "RelatedDependentProgramId",
                table: "DependentPrograms",
                newName: "ProtocolProgramId");

            migrationBuilder.RenameIndex(
                name: "IX_DependentPrograms_RelatedDependentProgramId",
                table: "DependentPrograms",
                newName: "IX_DependentPrograms_ProtocolProgramId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelingDate",
                table: "ProtocolPrograms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelingProtocolNo",
                table: "ProtocolPrograms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "ProtocolPrograms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProtocolDate",
                table: "ProtocolPrograms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DependentPrograms_ProtocolPrograms_ProtocolProgramId",
                table: "DependentPrograms",
                column: "ProtocolProgramId",
                principalTable: "ProtocolPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

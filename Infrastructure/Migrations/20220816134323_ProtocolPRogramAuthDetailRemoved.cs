using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class ProtocolPRogramAuthDetailRemoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorizationDetails_ProtocolPrograms_ProtocolProgramId",
                table: "AuthorizationDetails");

            migrationBuilder.DropIndex(
                name: "IX_AuthorizationDetails_ProtocolProgramId",
                table: "AuthorizationDetails");

            migrationBuilder.DropColumn(
                name: "ProtocolProgramId",
                table: "AuthorizationDetails");

            migrationBuilder.AddColumn<DateTime>(
                name: "ProtocolDate",
                table: "ProtocolPrograms",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProtocolDate",
                table: "ProtocolPrograms");

            migrationBuilder.AddColumn<long>(
                name: "ProtocolProgramId",
                table: "AuthorizationDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizationDetails_ProtocolProgramId",
                table: "AuthorizationDetails",
                column: "ProtocolProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorizationDetails_ProtocolPrograms_ProtocolProgramId",
                table: "AuthorizationDetails",
                column: "ProtocolProgramId",
                principalTable: "ProtocolPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

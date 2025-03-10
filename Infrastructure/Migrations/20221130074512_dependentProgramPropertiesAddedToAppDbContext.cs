using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class dependentProgramPropertiesAddedToAppDbContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DependentPrograms_Programs_ProgramId",
                table: "DependentPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_DependentPrograms_ProtocolPrograms_ProtocolProgramId",
                table: "DependentPrograms");

            migrationBuilder.AddForeignKey(
                name: "FK_DependentPrograms_Programs_ProgramId",
                table: "DependentPrograms",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_DependentPrograms_ProtocolPrograms_ProtocolProgramId",
                table: "DependentPrograms",
                column: "ProtocolProgramId",
                principalTable: "ProtocolPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DependentPrograms_Programs_ProgramId",
                table: "DependentPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_DependentPrograms_ProtocolPrograms_ProtocolProgramId",
                table: "DependentPrograms");

            migrationBuilder.AddForeignKey(
                name: "FK_DependentPrograms_Programs_ProgramId",
                table: "DependentPrograms",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DependentPrograms_ProtocolPrograms_ProtocolProgramId",
                table: "DependentPrograms",
                column: "ProtocolProgramId",
                principalTable: "ProtocolPrograms",
                principalColumn: "Id");
        }
    }
}

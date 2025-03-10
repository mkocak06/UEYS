using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class unitAddedToDependentProgram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProtocolProgramStudent",
                table: "Students");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "DependentPrograms",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_ProtocolProgramId",
                table: "Students",
                column: "ProtocolProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Programs_ProtocolProgramId",
                table: "Students",
                column: "ProtocolProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Programs_ProtocolProgramId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_ProtocolProgramId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "DependentPrograms");

            migrationBuilder.AddColumn<bool>(
                name: "IsProtocolProgramStudent",
                table: "Students",
                type: "boolean",
                nullable: true);
        }
    }
}

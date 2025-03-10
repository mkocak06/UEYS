using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class protocolProgramAddedToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProtocolProgramStudent",
                table: "Students",
                type: "boolean",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_OriginalProgramId",
                table: "Students",
                column: "OriginalProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Programs_OriginalProgramId",
                table: "Students",
                column: "OriginalProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Programs_OriginalProgramId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_OriginalProgramId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "IsProtocolProgramStudent",
                table: "Students");
        }
    }
}

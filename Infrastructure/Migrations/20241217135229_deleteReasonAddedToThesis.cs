using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class deleteReasonAddedToThesis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_asistan_hekim",
                schema: "kds_integration",
                table: "asistan_hekim");

            migrationBuilder.RenameTable(
                name: "asistan_hekim",
                schema: "kds_integration",
                newName: "hekim_portfoy_bilgileri",
                newSchema: "kds_integration");

            migrationBuilder.RenameColumn(
                name: "asistan_hekim_kimlik_numarasi",
                schema: "kds_integration",
                table: "hekim_portfoy_bilgileri",
                newName: "hekim_kimlik_numarasi");

            migrationBuilder.AddColumn<string>(
                name: "DeleteReasonExplanation",
                table: "Theses",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_hekim_portfoy_bilgileri",
                schema: "kds_integration",
                table: "hekim_portfoy_bilgileri",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_hekim_portfoy_bilgileri",
                schema: "kds_integration",
                table: "hekim_portfoy_bilgileri");

            migrationBuilder.DropColumn(
                name: "DeleteReasonExplanation",
                table: "Theses");

            migrationBuilder.RenameTable(
                name: "hekim_portfoy_bilgileri",
                schema: "kds_integration",
                newName: "asistan_hekim",
                newSchema: "kds_integration");

            migrationBuilder.RenameColumn(
                name: "hekim_kimlik_numarasi",
                schema: "kds_integration",
                table: "asistan_hekim",
                newName: "asistan_hekim_kimlik_numarasi");

            migrationBuilder.AddPrimaryKey(
                name: "PK_asistan_hekim",
                schema: "kds_integration",
                table: "asistan_hekim",
                column: "id");
        }
    }
}

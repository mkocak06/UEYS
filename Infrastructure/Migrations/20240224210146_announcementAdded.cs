using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class announcementAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "StudentCounts");

            migrationBuilder.DropColumn(
                name: "IsRequest",
                table: "StudentCounts");

            migrationBuilder.AddColumn<int>(
                name: "EducatorCount",
                table: "SubQuotaRequests",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AllocatedCount",
                table: "StudentCounts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestedCount",
                table: "StudentCounts",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Sina",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IlAdi = table.Column<string>(type: "text", nullable: true),
                    UstKurumAdi = table.Column<string>(type: "text", nullable: true),
                    KurumAdi = table.Column<string>(type: "text", nullable: true),
                    FakulteTipi = table.Column<string>(type: "text", nullable: true),
                    FakulteAdi = table.Column<string>(type: "text", nullable: true),
                    EgitimVerilenKurumAdi = table.Column<string>(type: "text", nullable: true),
                    EgiticiSayisi = table.Column<int>(type: "integer", nullable: false),
                    OgrenciSayisi = table.Column<int>(type: "integer", nullable: false),
                    ProgramSayisi = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sina", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sina");

            migrationBuilder.DropColumn(
                name: "EducatorCount",
                table: "SubQuotaRequests");

            migrationBuilder.DropColumn(
                name: "AllocatedCount",
                table: "StudentCounts");

            migrationBuilder.DropColumn(
                name: "RequestedCount",
                table: "StudentCounts");

            migrationBuilder.AddColumn<string>(
                name: "Count",
                table: "StudentCounts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequest",
                table: "StudentCounts",
                type: "boolean",
                nullable: true);
        }
    }
}

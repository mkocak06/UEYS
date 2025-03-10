using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class expBrIdAddedToAdvisorThesis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "kds_integration");

            migrationBuilder.AddColumn<long>(
                name: "ExpertiseBranchId",
                table: "AdvisorTheses",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "asistan_hekim",
                schema: "kds_integration",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    yilay = table.Column<string>(type: "text", nullable: true),
                    asistan_hekim_kimlik_numarasi = table.Column<string>(type: "text", nullable: true),
                    hizmet_sunucu = table.Column<string>(type: "text", nullable: true),
                    kurum_adi = table.Column<string>(type: "text", nullable: true),
                    klinik_kodu = table.Column<string>(type: "text", nullable: true),
                    klinik_adi = table.Column<string>(type: "text", nullable: true),
                    il_kodu = table.Column<string>(type: "text", nullable: true),
                    il_adi = table.Column<string>(type: "text", nullable: true),
                    ilce_kodu = table.Column<int>(type: "integer", nullable: true),
                    islem_sayisi = table.Column<int>(type: "integer", nullable: true),
                    muayene_sayisi = table.Column<int>(type: "integer", nullable: true),
                    ameliyat_ve_girisimler_sayisi = table.Column<int>(type: "integer", nullable: true),
                    diger_islemler_sayisi = table.Column<int>(type: "integer", nullable: true),
                    dis_islemleri_sayisi = table.Column<int>(type: "integer", nullable: true),
                    dogum_islemleri_sayisi = table.Column<int>(type: "integer", nullable: true),
                    kan_islemleri_sayisi = table.Column<int>(type: "integer", nullable: true),
                    malzeme_sayisi = table.Column<int>(type: "integer", nullable: true),
                    tahlil_tetkik_ve_radyoloji_islemleri_sayisi = table.Column<int>(type: "integer", nullable: true),
                    yatak_islemleri_sayisi = table.Column<int>(type: "integer", nullable: true),
                    a_grubu_ameliyat_sayisi = table.Column<int>(type: "integer", nullable: true),
                    b_grubu_ameliyat_sayisi = table.Column<int>(type: "integer", nullable: true),
                    c_grubu_ameliyat_sayisi = table.Column<int>(type: "integer", nullable: true),
                    d_grubu_ameliyat_sayisi = table.Column<int>(type: "integer", nullable: true),
                    e_grubu_ameliyat_sayisi = table.Column<int>(type: "integer", nullable: true),
                    recete_sayisi = table.Column<int>(type: "integer", nullable: true),
                    ilac_sayisi = table.Column<int>(type: "integer", nullable: true),
                    guncelleme_zamani = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asistan_hekim", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdvisorTheses_ExpertiseBranchId",
                table: "AdvisorTheses",
                column: "ExpertiseBranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdvisorTheses_ExpertiseBranches_ExpertiseBranchId",
                table: "AdvisorTheses",
                column: "ExpertiseBranchId",
                principalTable: "ExpertiseBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvisorTheses_ExpertiseBranches_ExpertiseBranchId",
                table: "AdvisorTheses");

            migrationBuilder.DropTable(
                name: "asistan_hekim",
                schema: "kds_integration");

            migrationBuilder.DropIndex(
                name: "IX_AdvisorTheses_ExpertiseBranchId",
                table: "AdvisorTheses");

            migrationBuilder.DropColumn(
                name: "ExpertiseBranchId",
                table: "AdvisorTheses");
        }
    }
}

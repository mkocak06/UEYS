using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class sinaUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OgrenciSayisi",
                table: "Sina");

            migrationBuilder.DropColumn(
                name: "ProgramSayisi",
                table: "Sina");

            migrationBuilder.DropColumn(
                name: "SonAltiAyiKalanOgrenciSayisi",
                table: "Sina");

            migrationBuilder.AddColumn<string>(
                name: "BirlikteKullanimYapilanFakulte",
                table: "Sina",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BirlikteKullanimYapilanFakulteKodu",
                table: "Sina",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BirlikteKullanimYapilanKurum",
                table: "Sina",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BirlikteKullanimYapilanKurumKodu",
                table: "Sina",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EgitimVerilenKurumKodu",
                table: "Sina",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FakulteKodu",
                table: "Sina",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KurumKodu",
                table: "Sina",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime?[]>(
                name: "OgrenciMezuniyetTarihiListesi",
                table: "Sina",
                type: "timestamp with time zone[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UzmanlikDali",
                table: "Sina",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UzmanlikDaliAnaDalMi",
                table: "Sina",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UzmanlikDaliKodu",
                table: "Sina",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YetkiKategorisi",
                table: "Sina",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BirlikteKullanimYapilanFakulte",
                table: "Sina");

            migrationBuilder.DropColumn(
                name: "BirlikteKullanimYapilanFakulteKodu",
                table: "Sina");

            migrationBuilder.DropColumn(
                name: "BirlikteKullanimYapilanKurum",
                table: "Sina");

            migrationBuilder.DropColumn(
                name: "BirlikteKullanimYapilanKurumKodu",
                table: "Sina");

            migrationBuilder.DropColumn(
                name: "EgitimVerilenKurumKodu",
                table: "Sina");

            migrationBuilder.DropColumn(
                name: "FakulteKodu",
                table: "Sina");

            migrationBuilder.DropColumn(
                name: "KurumKodu",
                table: "Sina");

            migrationBuilder.DropColumn(
                name: "OgrenciMezuniyetTarihiListesi",
                table: "Sina");

            migrationBuilder.DropColumn(
                name: "UzmanlikDali",
                table: "Sina");

            migrationBuilder.DropColumn(
                name: "UzmanlikDaliAnaDalMi",
                table: "Sina");

            migrationBuilder.DropColumn(
                name: "UzmanlikDaliKodu",
                table: "Sina");

            migrationBuilder.DropColumn(
                name: "YetkiKategorisi",
                table: "Sina");

            migrationBuilder.AddColumn<int>(
                name: "OgrenciSayisi",
                table: "Sina",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProgramSayisi",
                table: "Sina",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SonAltiAyiKalanOgrenciSayisi",
                table: "Sina",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}

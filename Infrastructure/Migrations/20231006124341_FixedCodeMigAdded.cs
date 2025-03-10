using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixedCodeMigAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GraduationDetails_Educators_EducatorId",
                table: "GraduationDetails");

            migrationBuilder.AddColumn<int>(
                name: "Code",
                table: "Provinces",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "Code",
                table: "Professions",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "Code",
                table: "Hospitals",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "Code",
                table: "ExpertiseBranches",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_GraduationDetails_Educators_EducatorId",
                table: "GraduationDetails",
                column: "EducatorId",
                principalTable: "Educators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GraduationDetails_Educators_EducatorId",
                table: "GraduationDetails");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Provinces");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Professions");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "ExpertiseBranches");

            migrationBuilder.AddForeignKey(
                name: "FK_GraduationDetails_Educators_EducatorId",
                table: "GraduationDetails",
                column: "EducatorId",
                principalTable: "Educators",
                principalColumn: "Id");
        }
    }
}

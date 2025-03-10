using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GraduationDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GraduationDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HigherEducationDetail = table.Column<string>(type: "text", nullable: true),
                    GraduationUniversity = table.Column<string>(type: "text", nullable: true),
                    GraduationFaculty = table.Column<string>(type: "text", nullable: true),
                    GraduationField = table.Column<string>(type: "text", nullable: true),
                    GraduationDate = table.Column<string>(type: "text", nullable: true),
                    EducatorId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraduationDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GraduationDetails_Educators_EducatorId",
                        column: x => x.EducatorId,
                        principalTable: "Educators",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GraduationDetails_EducatorId",
                table: "GraduationDetails",
                column: "EducatorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GraduationDetails");
        }
    }
}

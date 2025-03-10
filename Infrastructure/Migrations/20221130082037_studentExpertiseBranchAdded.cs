using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class studentExpertiseBranchAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentExpertiseBranches",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RegistrationDate = table.Column<string>(type: "text", nullable: true),
                    ExpertiseBranchId = table.Column<long>(type: "bigint", nullable: true),
                    StudentId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentExpertiseBranches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentExpertiseBranches_ExpertiseBranches_ExpertiseBranchId",
                        column: x => x.ExpertiseBranchId,
                        principalTable: "ExpertiseBranches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentExpertiseBranches_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentExpertiseBranches_ExpertiseBranchId",
                table: "StudentExpertiseBranches",
                column: "ExpertiseBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentExpertiseBranches_StudentId",
                table: "StudentExpertiseBranches",
                column: "StudentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentExpertiseBranches");
        }
    }
}

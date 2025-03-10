using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class PerfectionRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpertiseBranches_ExpertiseBranches_PrincipalBranchId",
                table: "ExpertiseBranches");

            migrationBuilder.DropIndex(
                name: "IX_ExpertiseBranches_PrincipalBranchId",
                table: "ExpertiseBranches");

            migrationBuilder.DropColumn(
                name: "PerfectionGroup",
                table: "Perfections");

            migrationBuilder.DropColumn(
                name: "PerfectionLevel",
                table: "Perfections");

            migrationBuilder.DropColumn(
                name: "PerfectionMethod",
                table: "Perfections");

            migrationBuilder.DropColumn(
                name: "PerfectionName",
                table: "Perfections");

            migrationBuilder.DropColumn(
                name: "PrincipalBranchId",
                table: "ExpertiseBranches");

            migrationBuilder.RenameColumn(
                name: "PerfectionSeniorty",
                table: "Perfections",
                newName: "Name");

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    PropertyType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RelatedExpertiseBranches",
                columns: table => new
                {
                    PrincipalBranchId = table.Column<long>(type: "bigint", nullable: false),
                    SubBranchId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatedExpertiseBranches", x => new { x.PrincipalBranchId, x.SubBranchId });
                    table.ForeignKey(
                        name: "FK_RelatedExpertiseBranches_ExpertiseBranches_PrincipalBranchId",
                        column: x => x.PrincipalBranchId,
                        principalTable: "ExpertiseBranches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RelatedExpertiseBranches_ExpertiseBranches_SubBranchId",
                        column: x => x.SubBranchId,
                        principalTable: "ExpertiseBranches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PerfectionProperties",
                columns: table => new
                {
                    PerfectionId = table.Column<long>(type: "bigint", nullable: false),
                    PropertyId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfectionProperties", x => new { x.PropertyId, x.PerfectionId });
                    table.ForeignKey(
                        name: "FK_PerfectionProperties_Perfections_PerfectionId",
                        column: x => x.PerfectionId,
                        principalTable: "Perfections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PerfectionProperties_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PerfectionProperties_PerfectionId",
                table: "PerfectionProperties",
                column: "PerfectionId");

            migrationBuilder.CreateIndex(
                name: "IX_RelatedExpertiseBranches_SubBranchId",
                table: "RelatedExpertiseBranches",
                column: "SubBranchId");

            migrationBuilder.Sql("ALTER TABLE \"Perfections\" ALTER COLUMN \"PerfectionType\" TYPE integer USING(\"PerfectionType\"::integer);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PerfectionProperties");

            migrationBuilder.DropTable(
                name: "RelatedExpertiseBranches");

            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Perfections",
                newName: "PerfectionSeniorty");

            migrationBuilder.AddColumn<string>(
                name: "PerfectionGroup",
                table: "Perfections",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PerfectionLevel",
                table: "Perfections",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PerfectionMethod",
                table: "Perfections",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PerfectionName",
                table: "Perfections",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PrincipalBranchId",
                table: "ExpertiseBranches",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExpertiseBranches_PrincipalBranchId",
                table: "ExpertiseBranches",
                column: "PrincipalBranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpertiseBranches_ExpertiseBranches_PrincipalBranchId",
                table: "ExpertiseBranches",
                column: "PrincipalBranchId",
                principalTable: "ExpertiseBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

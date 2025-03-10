using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class ethicCommitteeDecisionAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EthicCommitteeDecision",
                table: "Theses");

            migrationBuilder.DropColumn(
                name: "EthicCommitteeDecisionNo",
                table: "Theses");

            migrationBuilder.RenameColumn(
                name: "EthicCommitteeDecisionDate",
                table: "Theses",
                newName: "ThesisTitleDetermineDate");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProgressReports",
                newName: "Description");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Theses",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "FinalExamResult",
                table: "Theses",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThesisSubjectType_1",
                table: "Theses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThesisSubjectType_2",
                table: "Theses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsProgramManager",
                table: "EducatorDependentPrograms",
                type: "boolean",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EthicCommitteeDecisions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Number = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ThesisId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EthicCommitteeDecisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EthicCommitteeDecisions_Theses_ThesisId",
                        column: x => x.ThesisId,
                        principalTable: "Theses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EthicCommitteeDecisions_ThesisId",
                table: "EthicCommitteeDecisions",
                column: "ThesisId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EthicCommitteeDecisions");

            migrationBuilder.DropColumn(
                name: "FinalExamResult",
                table: "Theses");

            migrationBuilder.DropColumn(
                name: "ThesisSubjectType_1",
                table: "Theses");

            migrationBuilder.DropColumn(
                name: "ThesisSubjectType_2",
                table: "Theses");

            migrationBuilder.DropColumn(
                name: "IsProgramManager",
                table: "EducatorDependentPrograms");

            migrationBuilder.RenameColumn(
                name: "ThesisTitleDetermineDate",
                table: "Theses",
                newName: "EthicCommitteeDecisionDate");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "ProgressReports",
                newName: "Name");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Theses",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EthicCommitteeDecision",
                table: "Theses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EthicCommitteeDecisionNo",
                table: "Theses",
                type: "text",
                nullable: true);
        }
    }
}

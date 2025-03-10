using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class transferRemoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transfers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transfers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequestCompleteUserId = table.Column<long>(type: "bigint", nullable: true),
                    RequestCreateUserId = table.Column<long>(type: "bigint", nullable: true),
                    StudentId = table.Column<long>(type: "bigint", nullable: true),
                    AdditionalExplanation = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsSpouseSpecializationStudent = table.Column<bool>(type: "boolean", nullable: true),
                    Reason = table.Column<int>(type: "integer", nullable: true),
                    RequestFinishDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SpouseIdentityNo = table.Column<string>(type: "text", nullable: true),
                    SpouseJob = table.Column<string>(type: "text", nullable: true),
                    SpouseName = table.Column<string>(type: "text", nullable: true),
                    TUKDecisionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TUKDecisionNumber = table.Column<string>(type: "text", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transfers_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Transfers_Users_RequestCompleteUserId",
                        column: x => x.RequestCompleteUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Transfers_Users_RequestCreateUserId",
                        column: x => x.RequestCreateUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_RequestCompleteUserId",
                table: "Transfers",
                column: "RequestCompleteUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_RequestCreateUserId",
                table: "Transfers",
                column: "RequestCreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_StudentId",
                table: "Transfers",
                column: "StudentId");
        }
    }
}

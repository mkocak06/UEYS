using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _795changesMade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ContributionCoefficientForEducatorIndex",
                table: "Titles",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "EducatorIndexRateToCapacityIndex",
                table: "ExpertiseBranches",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PortfolioIndexRateToCapacityIndex",
                table: "ExpertiseBranches",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsQuotaRequestable",
                table: "AuthorizationCategories",
                type: "boolean",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EducatorCountContributionFormulas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MinEducatorCount = table.Column<int>(type: "integer", nullable: true),
                    MaxEducatorCount = table.Column<int>(type: "integer", nullable: true),
                    Coefficient = table.Column<double>(type: "double precision", nullable: true),
                    BaseScore = table.Column<double>(type: "double precision", nullable: true),
                    IsExpert = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducatorCountContributionFormulas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Portfolios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Ratio = table.Column<double>(type: "double precision", nullable: true),
                    ExpertiseBranchId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Portfolios_ExpertiseBranches_ExpertiseBranchId",
                        column: x => x.ExpertiseBranchId,
                        principalTable: "ExpertiseBranches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "QuotaRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Period = table.Column<int>(type: "integer", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    TUKDecisionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TUKDecisionNumber = table.Column<string>(type: "text", nullable: true),
                    GlobalQuota = table.Column<int>(type: "integer", nullable: true),
                    HospitalId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotaRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuotaRequests_Hospitals_HospitalId",
                        column: x => x.HospitalId,
                        principalTable: "Hospitals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SubQuotaRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProgramId = table.Column<long>(type: "bigint", nullable: true),
                    QuotaRequestId = table.Column<long>(type: "bigint", nullable: true),
                    EducatorIndex = table.Column<int>(type: "integer", nullable: true),
                    PortfolioIndex = table.Column<int>(type: "integer", nullable: true),
                    CapacityIndex = table.Column<int>(type: "integer", nullable: true),
                    Capacity = table.Column<int>(type: "integer", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubQuotaRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubQuotaRequests_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SubQuotaRequests_QuotaRequests_QuotaRequestId",
                        column: x => x.QuotaRequestId,
                        principalTable: "QuotaRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "StudentCounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Count = table.Column<string>(type: "text", nullable: true),
                    QuotaType = table.Column<int>(type: "integer", nullable: true),
                    IsRequest = table.Column<bool>(type: "boolean", nullable: true),
                    SubQuotaRequestId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentCounts_SubQuotaRequests_SubQuotaRequestId",
                        column: x => x.SubQuotaRequestId,
                        principalTable: "SubQuotaRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SubQuotaRequestPortfolios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Answer = table.Column<int>(type: "integer", nullable: true),
                    PortfolioId = table.Column<long>(type: "bigint", nullable: true),
                    SubQuotaRequestId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubQuotaRequestPortfolios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubQuotaRequestPortfolios_Portfolios_PortfolioId",
                        column: x => x.PortfolioId,
                        principalTable: "Portfolios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SubQuotaRequestPortfolios_SubQuotaRequests_SubQuotaRequestId",
                        column: x => x.SubQuotaRequestId,
                        principalTable: "SubQuotaRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_ExpertiseBranchId",
                table: "Portfolios",
                column: "ExpertiseBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotaRequests_HospitalId",
                table: "QuotaRequests",
                column: "HospitalId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCounts_SubQuotaRequestId",
                table: "StudentCounts",
                column: "SubQuotaRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_SubQuotaRequestPortfolios_PortfolioId",
                table: "SubQuotaRequestPortfolios",
                column: "PortfolioId");

            migrationBuilder.CreateIndex(
                name: "IX_SubQuotaRequestPortfolios_SubQuotaRequestId",
                table: "SubQuotaRequestPortfolios",
                column: "SubQuotaRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_SubQuotaRequests_ProgramId",
                table: "SubQuotaRequests",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_SubQuotaRequests_QuotaRequestId",
                table: "SubQuotaRequests",
                column: "QuotaRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EducatorCountContributionFormulas");

            migrationBuilder.DropTable(
                name: "StudentCounts");

            migrationBuilder.DropTable(
                name: "SubQuotaRequestPortfolios");

            migrationBuilder.DropTable(
                name: "Portfolios");

            migrationBuilder.DropTable(
                name: "SubQuotaRequests");

            migrationBuilder.DropTable(
                name: "QuotaRequests");

            migrationBuilder.DropColumn(
                name: "ContributionCoefficientForEducatorIndex",
                table: "Titles");

            migrationBuilder.DropColumn(
                name: "EducatorIndexRateToCapacityIndex",
                table: "ExpertiseBranches");

            migrationBuilder.DropColumn(
                name: "PortfolioIndexRateToCapacityIndex",
                table: "ExpertiseBranches");

            migrationBuilder.DropColumn(
                name: "IsQuotaRequestable",
                table: "AuthorizationCategories");
        }
    }
}

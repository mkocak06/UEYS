using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VisitModuleModified5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forms_AuthorizationCategories_AuthorityCategoryId",
                table: "Forms");

            migrationBuilder.DropForeignKey(
                name: "FK_Forms_ExpertiseBranches_ExpertiseBranchId",
                table: "Forms");

            migrationBuilder.DropForeignKey(
                name: "FK_Standards_StandardCategories_CategoryId",
                table: "Standards");

            migrationBuilder.DropTable(
                name: "AuditCommittees");

            migrationBuilder.DropTable(
                name: "Decisions");

            migrationBuilder.DropTable(
                name: "MinRequirementForms");

            migrationBuilder.DropTable(
                name: "MinRequirements");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Standards",
                newName: "StandardCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Standards_CategoryId",
                table: "Standards",
                newName: "IX_Standards_StandardCategoryId");

            migrationBuilder.RenameColumn(
                name: "ExpertiseBranchId",
                table: "Forms",
                newName: "ProgramId");

            migrationBuilder.RenameColumn(
                name: "AuthorityCategoryId",
                table: "Forms",
                newName: "AuthorizationDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_Forms_ExpertiseBranchId",
                table: "Forms",
                newName: "IX_Forms_ProgramId");

            migrationBuilder.RenameIndex(
                name: "IX_Forms_AuthorityCategoryId",
                table: "Forms",
                newName: "IX_Forms_AuthorizationDetailId");

            migrationBuilder.AddColumn<long>(
                name: "ExpertiseBranchId",
                table: "Standards",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "CommitteeOpinionType",
                table: "Forms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FormStandards",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InstitutionStatement = table.Column<bool>(type: "boolean", nullable: false),
                    CommitteeStatement = table.Column<bool>(type: "boolean", nullable: false),
                    CommitteeDescription = table.Column<string>(type: "text", nullable: true),
                    StandardId = table.Column<long>(type: "bigint", nullable: false),
                    FormId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormStandards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormStandards_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FormStandards_Standards_StandardId",
                        column: x => x.StandardId,
                        principalTable: "Standards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OnSiteVisitCommittees",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    FormId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnSiteVisitCommittees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnSiteVisitCommittees_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OnSiteVisitCommittees_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Standards_ExpertiseBranchId",
                table: "Standards",
                column: "ExpertiseBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_FormStandards_FormId",
                table: "FormStandards",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_FormStandards_StandardId",
                table: "FormStandards",
                column: "StandardId");

            migrationBuilder.CreateIndex(
                name: "IX_OnSiteVisitCommittees_FormId",
                table: "OnSiteVisitCommittees",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_OnSiteVisitCommittees_UserId",
                table: "OnSiteVisitCommittees",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_AuthorizationDetails_AuthorizationDetailId",
                table: "Forms",
                column: "AuthorizationDetailId",
                principalTable: "AuthorizationDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_Programs_ProgramId",
                table: "Forms",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Standards_ExpertiseBranches_ExpertiseBranchId",
                table: "Standards",
                column: "ExpertiseBranchId",
                principalTable: "ExpertiseBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Standards_StandardCategories_StandardCategoryId",
                table: "Standards",
                column: "StandardCategoryId",
                principalTable: "StandardCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forms_AuthorizationDetails_AuthorizationDetailId",
                table: "Forms");

            migrationBuilder.DropForeignKey(
                name: "FK_Forms_Programs_ProgramId",
                table: "Forms");

            migrationBuilder.DropForeignKey(
                name: "FK_Standards_ExpertiseBranches_ExpertiseBranchId",
                table: "Standards");

            migrationBuilder.DropForeignKey(
                name: "FK_Standards_StandardCategories_StandardCategoryId",
                table: "Standards");

            migrationBuilder.DropTable(
                name: "FormStandards");

            migrationBuilder.DropTable(
                name: "OnSiteVisitCommittees");

            migrationBuilder.DropIndex(
                name: "IX_Standards_ExpertiseBranchId",
                table: "Standards");

            migrationBuilder.DropColumn(
                name: "ExpertiseBranchId",
                table: "Standards");

            migrationBuilder.DropColumn(
                name: "CommitteeOpinionType",
                table: "Forms");

            migrationBuilder.RenameColumn(
                name: "StandardCategoryId",
                table: "Standards",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Standards_StandardCategoryId",
                table: "Standards",
                newName: "IX_Standards_CategoryId");

            migrationBuilder.RenameColumn(
                name: "ProgramId",
                table: "Forms",
                newName: "ExpertiseBranchId");

            migrationBuilder.RenameColumn(
                name: "AuthorizationDetailId",
                table: "Forms",
                newName: "AuthorityCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Forms_ProgramId",
                table: "Forms",
                newName: "IX_Forms_ExpertiseBranchId");

            migrationBuilder.RenameIndex(
                name: "IX_Forms_AuthorizationDetailId",
                table: "Forms",
                newName: "IX_Forms_AuthorityCategoryId");

            migrationBuilder.CreateTable(
                name: "AuditCommittees",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FormId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditCommittees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditCommittees_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AuditCommittees_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Decisions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FormId = table.Column<long>(type: "bigint", nullable: false),
                    AvaibilityStatusType = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DecisionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DecisionType = table.Column<int>(type: "integer", nullable: false),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Decisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Decisions_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "MinRequirements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StandartId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MinRequirements_Standards_StandartId",
                        column: x => x.StandartId,
                        principalTable: "Standards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "MinRequirementForms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FormId = table.Column<long>(type: "bigint", nullable: false),
                    MinRequirementId = table.Column<long>(type: "bigint", nullable: false),
                    RequirementStatusType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinRequirementForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MinRequirementForms_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MinRequirementForms_MinRequirements_MinRequirementId",
                        column: x => x.MinRequirementId,
                        principalTable: "MinRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditCommittees_FormId",
                table: "AuditCommittees",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditCommittees_UserId",
                table: "AuditCommittees",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Decisions_FormId",
                table: "Decisions",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_MinRequirementForms_FormId",
                table: "MinRequirementForms",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_MinRequirementForms_MinRequirementId",
                table: "MinRequirementForms",
                column: "MinRequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_MinRequirements_StandartId",
                table: "MinRequirements",
                column: "StandartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_AuthorizationCategories_AuthorityCategoryId",
                table: "Forms",
                column: "AuthorityCategoryId",
                principalTable: "AuthorizationCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_ExpertiseBranches_ExpertiseBranchId",
                table: "Forms",
                column: "ExpertiseBranchId",
                principalTable: "ExpertiseBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Standards_StandardCategories_CategoryId",
                table: "Standards",
                column: "CategoryId",
                principalTable: "StandardCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

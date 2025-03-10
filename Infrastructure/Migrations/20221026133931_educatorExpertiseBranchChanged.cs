using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class educatorExpertiseBranchChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducatorExpertiseBranches_Educators_EducatorId",
                table: "EducatorExpertiseBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_EducatorExpertiseBranches_ExpertiseBranches_ExpertiseBranch~",
                table: "EducatorExpertiseBranches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EducatorExpertiseBranches",
                table: "EducatorExpertiseBranches");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "EducatorExpertiseBranches",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<long>(
                name: "EducatorId",
                table: "EducatorExpertiseBranches",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "ExpertiseBranchId",
                table: "EducatorExpertiseBranches",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EducatorExpertiseBranches",
                table: "EducatorExpertiseBranches",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EducatorExpertiseBranches_ExpertiseBranchId",
                table: "EducatorExpertiseBranches",
                column: "ExpertiseBranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorExpertiseBranches_Educators_EducatorId",
                table: "EducatorExpertiseBranches",
                column: "EducatorId",
                principalTable: "Educators",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorExpertiseBranches_ExpertiseBranches_ExpertiseBranch~",
                table: "EducatorExpertiseBranches",
                column: "ExpertiseBranchId",
                principalTable: "ExpertiseBranches",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducatorExpertiseBranches_Educators_EducatorId",
                table: "EducatorExpertiseBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_EducatorExpertiseBranches_ExpertiseBranches_ExpertiseBranch~",
                table: "EducatorExpertiseBranches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EducatorExpertiseBranches",
                table: "EducatorExpertiseBranches");

            migrationBuilder.DropIndex(
                name: "IX_EducatorExpertiseBranches_ExpertiseBranchId",
                table: "EducatorExpertiseBranches");

            migrationBuilder.AlterColumn<long>(
                name: "ExpertiseBranchId",
                table: "EducatorExpertiseBranches",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "EducatorId",
                table: "EducatorExpertiseBranches",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "EducatorExpertiseBranches",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EducatorExpertiseBranches",
                table: "EducatorExpertiseBranches",
                columns: new[] { "ExpertiseBranchId", "EducatorId" });

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorExpertiseBranches_Educators_EducatorId",
                table: "EducatorExpertiseBranches",
                column: "EducatorId",
                principalTable: "Educators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EducatorExpertiseBranches_ExpertiseBranches_ExpertiseBranch~",
                table: "EducatorExpertiseBranches",
                column: "ExpertiseBranchId",
                principalTable: "ExpertiseBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

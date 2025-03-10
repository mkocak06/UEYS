using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class specificeducationsaddedtotb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecificEducation_Curricula_CurriculumId",
                table: "SpecificEducation");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentsSpecificEducation_SpecificEducationPlace_SpecificEd~",
                table: "StudentsSpecificEducation");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentsSpecificEducation_SpecificEducation_SpecificEducati~",
                table: "StudentsSpecificEducation");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentsSpecificEducation_Students_StudentId",
                table: "StudentsSpecificEducation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentsSpecificEducation",
                table: "StudentsSpecificEducation");

            migrationBuilder.DropIndex(
                name: "IX_StudentsSpecificEducation_SpecificEducationId",
                table: "StudentsSpecificEducation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SpecificEducationPlace",
                table: "SpecificEducationPlace");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SpecificEducation",
                table: "SpecificEducation");

            migrationBuilder.DropColumn(
                name: "SpecificEducationId",
                table: "StudentsSpecificEducation");

            migrationBuilder.RenameTable(
                name: "StudentsSpecificEducation",
                newName: "StudentsSpecificEducations");

            migrationBuilder.RenameTable(
                name: "SpecificEducationPlace",
                newName: "SpecificEducationPlaces");

            migrationBuilder.RenameTable(
                name: "SpecificEducation",
                newName: "SpecificEducations");

            migrationBuilder.RenameIndex(
                name: "IX_StudentsSpecificEducation_StudentId",
                table: "StudentsSpecificEducations",
                newName: "IX_StudentsSpecificEducations_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentsSpecificEducation_SpecificEducationPlaceId",
                table: "StudentsSpecificEducations",
                newName: "IX_StudentsSpecificEducations_SpecificEducationPlaceId");

            migrationBuilder.RenameIndex(
                name: "IX_SpecificEducation_CurriculumId",
                table: "SpecificEducations",
                newName: "IX_SpecificEducations_CurriculumId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentsSpecificEducations",
                table: "StudentsSpecificEducations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SpecificEducationPlaces",
                table: "SpecificEducationPlaces",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SpecificEducations",
                table: "SpecificEducations",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StudentsSpecificEducations_SpesificEducationId",
                table: "StudentsSpecificEducations",
                column: "SpesificEducationId");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificEducations_Curricula_CurriculumId",
                table: "SpecificEducations",
                column: "CurriculumId",
                principalTable: "Curricula",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentsSpecificEducations_SpecificEducationPlaces_Specific~",
                table: "StudentsSpecificEducations",
                column: "SpecificEducationPlaceId",
                principalTable: "SpecificEducationPlaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentsSpecificEducations_SpecificEducations_SpesificEduca~",
                table: "StudentsSpecificEducations",
                column: "SpesificEducationId",
                principalTable: "SpecificEducations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentsSpecificEducations_Students_StudentId",
                table: "StudentsSpecificEducations",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecificEducations_Curricula_CurriculumId",
                table: "SpecificEducations");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentsSpecificEducations_SpecificEducationPlaces_Specific~",
                table: "StudentsSpecificEducations");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentsSpecificEducations_SpecificEducations_SpesificEduca~",
                table: "StudentsSpecificEducations");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentsSpecificEducations_Students_StudentId",
                table: "StudentsSpecificEducations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentsSpecificEducations",
                table: "StudentsSpecificEducations");

            migrationBuilder.DropIndex(
                name: "IX_StudentsSpecificEducations_SpesificEducationId",
                table: "StudentsSpecificEducations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SpecificEducations",
                table: "SpecificEducations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SpecificEducationPlaces",
                table: "SpecificEducationPlaces");

            migrationBuilder.RenameTable(
                name: "StudentsSpecificEducations",
                newName: "StudentsSpecificEducation");

            migrationBuilder.RenameTable(
                name: "SpecificEducations",
                newName: "SpecificEducation");

            migrationBuilder.RenameTable(
                name: "SpecificEducationPlaces",
                newName: "SpecificEducationPlace");

            migrationBuilder.RenameIndex(
                name: "IX_StudentsSpecificEducations_StudentId",
                table: "StudentsSpecificEducation",
                newName: "IX_StudentsSpecificEducation_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentsSpecificEducations_SpecificEducationPlaceId",
                table: "StudentsSpecificEducation",
                newName: "IX_StudentsSpecificEducation_SpecificEducationPlaceId");

            migrationBuilder.RenameIndex(
                name: "IX_SpecificEducations_CurriculumId",
                table: "SpecificEducation",
                newName: "IX_SpecificEducation_CurriculumId");

            migrationBuilder.AddColumn<long>(
                name: "SpecificEducationId",
                table: "StudentsSpecificEducation",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentsSpecificEducation",
                table: "StudentsSpecificEducation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SpecificEducation",
                table: "SpecificEducation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SpecificEducationPlace",
                table: "SpecificEducationPlace",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StudentsSpecificEducation_SpecificEducationId",
                table: "StudentsSpecificEducation",
                column: "SpecificEducationId");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificEducation_Curricula_CurriculumId",
                table: "SpecificEducation",
                column: "CurriculumId",
                principalTable: "Curricula",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentsSpecificEducation_SpecificEducationPlace_SpecificEd~",
                table: "StudentsSpecificEducation",
                column: "SpecificEducationPlaceId",
                principalTable: "SpecificEducationPlace",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentsSpecificEducation_SpecificEducation_SpecificEducati~",
                table: "StudentsSpecificEducation",
                column: "SpecificEducationId",
                principalTable: "SpecificEducation",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentsSpecificEducation_Students_StudentId",
                table: "StudentsSpecificEducation",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id");
        }
    }
}

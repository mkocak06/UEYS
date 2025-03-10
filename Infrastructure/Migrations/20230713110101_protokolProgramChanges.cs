using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class protokolProgramChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelingDate",
                table: "DependentPrograms");

            migrationBuilder.DropColumn(
                name: "CancelingProtocolNo",
                table: "DependentPrograms");

            migrationBuilder.DropColumn(
                name: "DecisionDate",
                table: "DependentPrograms");

            migrationBuilder.DropColumn(
                name: "DecisionNo",
                table: "DependentPrograms");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "DependentPrograms");

            migrationBuilder.DropColumn(
                name: "ProtocolDate",
                table: "DependentPrograms");

            migrationBuilder.DropColumn(
                name: "ProtocolNo",
                table: "DependentPrograms");

            migrationBuilder.AddColumn<DateTime>(
                name: "DecisionDate",
                table: "RelatedDependentProgram",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DecisionNo",
                table: "RelatedDependentProgram",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelingDate",
                table: "ProtocolPrograms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelingProtocolNo",
                table: "ProtocolPrograms",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DecisionDate",
                table: "RelatedDependentProgram");

            migrationBuilder.DropColumn(
                name: "DecisionNo",
                table: "RelatedDependentProgram");

            migrationBuilder.DropColumn(
                name: "CancelingDate",
                table: "ProtocolPrograms");

            migrationBuilder.DropColumn(
                name: "CancelingProtocolNo",
                table: "ProtocolPrograms");

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelingDate",
                table: "DependentPrograms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelingProtocolNo",
                table: "DependentPrograms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DecisionDate",
                table: "DependentPrograms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DecisionNo",
                table: "DependentPrograms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DocumentId",
                table: "DependentPrograms",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProtocolDate",
                table: "DependentPrograms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProtocolNo",
                table: "DependentPrograms",
                type: "text",
                nullable: true);
        }
    }
}

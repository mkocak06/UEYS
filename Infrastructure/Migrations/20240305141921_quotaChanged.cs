using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class quotaChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotaRequests_Hospitals_HospitalId",
                table: "QuotaRequests");

            migrationBuilder.RenameColumn(
                name: "GlobalQuota",
                table: "QuotaRequests",
                newName: "Type");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApplicationEndDate",
                table: "QuotaRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApplicationStartDate",
                table: "QuotaRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "QuotaRequests",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotaRequests_Hospitals_HospitalId",
                table: "QuotaRequests",
                column: "HospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotaRequests_Hospitals_HospitalId",
                table: "QuotaRequests");

            migrationBuilder.DropColumn(
                name: "ApplicationEndDate",
                table: "QuotaRequests");

            migrationBuilder.DropColumn(
                name: "ApplicationStartDate",
                table: "QuotaRequests");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "QuotaRequests");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "QuotaRequests",
                newName: "GlobalQuota");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotaRequests_Hospitals_HospitalId",
                table: "QuotaRequests",
                column: "HospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

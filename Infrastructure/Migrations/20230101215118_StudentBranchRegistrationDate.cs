using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class StudentBranchRegistrationDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"StudentExpertiseBranches\" ALTER COLUMN \"RegistrationDate\" TYPE timestamp with time zone USING(\"RegistrationDate\"::timestamp with time zone);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}

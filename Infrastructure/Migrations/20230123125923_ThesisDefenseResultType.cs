using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class ThesisDefenseResultType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"ThesisDefences\" ALTER COLUMN \"Result\" TYPE integer USING(\"Result\"::integer);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
        }
    }
}

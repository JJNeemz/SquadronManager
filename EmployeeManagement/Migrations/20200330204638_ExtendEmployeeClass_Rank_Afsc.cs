using Microsoft.EntityFrameworkCore.Migrations;

namespace EmployeeManagement.Migrations
{
    public partial class ExtendEmployeeClass_Rank_Afsc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Afsc",
                table: "Employees",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Employees",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Afsc",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Employees");
        }
    }
}

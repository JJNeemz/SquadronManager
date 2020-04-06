using Microsoft.EntityFrameworkCore.Migrations;

namespace EmployeeManagement.Migrations
{
    public partial class Employee_SeparateFirstAndLastName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Employees");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Employees",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Employees",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Mary", "Doe" });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "John", "Smith" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Employees");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Mary");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "John");
        }
    }
}

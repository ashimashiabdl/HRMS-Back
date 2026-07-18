using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Employee.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Awewpepts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountingSystemEmployeeId",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountingSystemEmployeeId",
                schema: "emp",
                table: "Employee");
        }
    }
}

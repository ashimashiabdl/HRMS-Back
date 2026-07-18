using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Employee.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Englishname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EnglishFirstName",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                defaultValueSql: "(N'')");

            migrationBuilder.AddColumn<string>(
                name: "EnglishLastName",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                defaultValueSql: "(N'')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnglishFirstName",
                schema: "emp",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "EnglishLastName",
                schema: "emp",
                table: "Employee");
        }
    }
}

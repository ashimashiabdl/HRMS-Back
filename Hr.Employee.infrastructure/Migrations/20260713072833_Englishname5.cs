using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Employee.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Englishname5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeathCause",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                defaultValueSql: "(N'')");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeathDate",
                schema: "emp",
                table: "Employee",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeathCause",
                schema: "emp",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "DeathDate",
                schema: "emp",
                table: "Employee");
        }
    }
}

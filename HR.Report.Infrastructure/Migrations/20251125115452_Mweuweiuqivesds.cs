using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Report.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Mweuweiuqivesds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InterdictOrder",
                schema: "rpt",
                table: "Employee_Property",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "InterdictOrderId",
                schema: "rpt",
                table: "Employee_Property",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterdictOrder",
                schema: "rpt",
                table: "Employee_Property");

            migrationBuilder.DropColumn(
                name: "InterdictOrderId",
                schema: "rpt",
                table: "Employee_Property");
        }
    }
}

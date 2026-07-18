using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Report.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Mweuzzzwexxiuqivesds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BaseTableId",
                schema: "rpt",
                table: "Reportable_Field",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseTableId",
                schema: "rpt",
                table: "Reportable_Field");
        }
    }
}

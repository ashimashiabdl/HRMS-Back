using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Report.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MicrossRepivesds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                schema: "rpt",
                table: "Reportable_Field",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                schema: "rpt",
                table: "Reportable_Field");
        }
    }
}

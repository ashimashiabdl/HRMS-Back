using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Report.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Mweuwexxiuqivesds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Property_Organisation_Chart_OrganisationChartId",
                schema: "rpt",
                table: "Employee_Property");

            migrationBuilder.DropIndex(
                name: "IX_Employee_Property_OrganisationChartId",
                schema: "rpt",
                table: "Employee_Property");

            migrationBuilder.DropColumn(
                name: "OrganisationChartId",
                schema: "rpt",
                table: "Employee_Property");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OrganisationChartId",
                schema: "rpt",
                table: "Employee_Property",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Property_OrganisationChartId",
                schema: "rpt",
                table: "Employee_Property",
                column: "OrganisationChartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Property_Organisation_Chart_OrganisationChartId",
                schema: "rpt",
                table: "Employee_Property",
                column: "OrganisationChartId",
                principalSchema: "Org",
                principalTable: "Organisation_Chart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

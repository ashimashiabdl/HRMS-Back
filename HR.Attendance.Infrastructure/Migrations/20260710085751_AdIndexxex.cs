using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Attendance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdIndexxex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OrganisationChartId",
                schema: "Attendance",
                table: "Attendance_Calendar",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_Calendar_OrganisationChartId",
                schema: "Attendance",
                table: "Attendance_Calendar",
                column: "OrganisationChartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendance_Calendar_Organisation_Chart_OrganisationChartId",
                schema: "Attendance",
                table: "Attendance_Calendar",
                column: "OrganisationChartId",
                principalSchema: "Org",
                principalTable: "Organisation_Chart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendance_Calendar_Organisation_Chart_OrganisationChartId",
                schema: "Attendance",
                table: "Attendance_Calendar");

            migrationBuilder.DropIndex(
                name: "IX_Attendance_Calendar_OrganisationChartId",
                schema: "Attendance",
                table: "Attendance_Calendar");

            migrationBuilder.DropColumn(
                name: "OrganisationChartId",
                schema: "Attendance",
                table: "Attendance_Calendar");
        }
    }
}

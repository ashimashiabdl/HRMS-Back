using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Attendance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOU : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RelatedOrganisationChartId",
                schema: "Attendance",
                table: "Attendance_Location",
                type: "bigint",
                nullable: true,
                comment: "واحد سازمانی مرتبط");

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_Location_RelatedOrganisationChartId",
                schema: "Attendance",
                table: "Attendance_Location",
                column: "RelatedOrganisationChartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendance_Location_Organisation_Chart_RelatedOrganisationChartId",
                schema: "Attendance",
                table: "Attendance_Location",
                column: "RelatedOrganisationChartId",
                principalSchema: "Org",
                principalTable: "Organisation_Chart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendance_Location_Organisation_Chart_RelatedOrganisationChartId",
                schema: "Attendance",
                table: "Attendance_Location");

            migrationBuilder.DropIndex(
                name: "IX_Attendance_Location_RelatedOrganisationChartId",
                schema: "Attendance",
                table: "Attendance_Location");

            migrationBuilder.DropColumn(
                name: "RelatedOrganisationChartId",
                schema: "Attendance",
                table: "Attendance_Location");
        }
    }
}

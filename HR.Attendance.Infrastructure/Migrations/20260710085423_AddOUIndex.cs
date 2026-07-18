using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Attendance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOUIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Attendance_Calendar_Date",
                schema: "Attendance",
                table: "Attendance_Calendar",
                column: "Date",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attendance_Calendar_Date",
                schema: "Attendance",
                table: "Attendance_Calendar");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Attendance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Shtrtrrrrr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHoliday",
                schema: "Attendance",
                table: "Attendance_Shift_Override_Detail",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "روز تعطیل");

            migrationBuilder.AddColumn<bool>(
                name: "IsHoliday",
                schema: "Attendance",
                table: "Attendance_Shift_Detail",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "روز تعطیل");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHoliday",
                schema: "Attendance",
                table: "Attendance_Shift_Override_Detail");

            migrationBuilder.DropColumn(
                name: "IsHoliday",
                schema: "Attendance",
                table: "Attendance_Shift_Detail");
        }
    }
}

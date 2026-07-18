using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Attendance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyResultInOutPairs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FifthIn",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result",
                type: "datetime",
                nullable: true,
                comment: "ورود پنجم");

            migrationBuilder.AddColumn<DateTime>(
                name: "FifthOut",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result",
                type: "datetime",
                nullable: true,
                comment: "خروج پنجم");

            migrationBuilder.AddColumn<DateTime>(
                name: "FourthIn",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result",
                type: "datetime",
                nullable: true,
                comment: "ورود چهارم");

            migrationBuilder.AddColumn<DateTime>(
                name: "FourthOut",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result",
                type: "datetime",
                nullable: true,
                comment: "خروج چهارم");

            migrationBuilder.AddColumn<DateTime>(
                name: "SecondIn",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result",
                type: "datetime",
                nullable: true,
                comment: "ورود دوم");

            migrationBuilder.AddColumn<DateTime>(
                name: "SecondOut",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result",
                type: "datetime",
                nullable: true,
                comment: "خروج دوم");

            migrationBuilder.AddColumn<DateTime>(
                name: "SeventhIn",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result",
                type: "datetime",
                nullable: true,
                comment: "ورود هفتم");

            migrationBuilder.AddColumn<DateTime>(
                name: "SeventhOut",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result",
                type: "datetime",
                nullable: true,
                comment: "خروج هفتم");

            migrationBuilder.AddColumn<DateTime>(
                name: "SixthIn",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result",
                type: "datetime",
                nullable: true,
                comment: "ورود ششم");

            migrationBuilder.AddColumn<DateTime>(
                name: "SixthOut",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result",
                type: "datetime",
                nullable: true,
                comment: "خروج ششم");

            migrationBuilder.AddColumn<DateTime>(
                name: "ThirdIn",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result",
                type: "datetime",
                nullable: true,
                comment: "ورود سوم");

            migrationBuilder.AddColumn<DateTime>(
                name: "ThirdOut",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result",
                type: "datetime",
                nullable: true,
                comment: "خروج سوم");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FifthIn",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result");

            migrationBuilder.DropColumn(
                name: "FifthOut",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result");

            migrationBuilder.DropColumn(
                name: "FourthIn",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result");

            migrationBuilder.DropColumn(
                name: "FourthOut",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result");

            migrationBuilder.DropColumn(
                name: "SecondIn",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result");

            migrationBuilder.DropColumn(
                name: "SecondOut",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result");

            migrationBuilder.DropColumn(
                name: "SeventhIn",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result");

            migrationBuilder.DropColumn(
                name: "SeventhOut",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result");

            migrationBuilder.DropColumn(
                name: "SixthIn",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result");

            migrationBuilder.DropColumn(
                name: "SixthOut",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result");

            migrationBuilder.DropColumn(
                name: "ThirdIn",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result");

            migrationBuilder.DropColumn(
                name: "ThirdOut",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result");
        }
    }
}

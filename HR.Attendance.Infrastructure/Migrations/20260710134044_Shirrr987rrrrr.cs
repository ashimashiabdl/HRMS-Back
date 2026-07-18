using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Attendance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Shirrr987rrrrr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Attendance_Exception_Attendance_Absence_Type_AbsenceJustificationId",
                schema: "Attendance",
                table: "Employee_Attendance_Exception");

            migrationBuilder.DropIndex(
                name: "IX_Employee_Attendance_Exception_AbsenceJustificationId",
                schema: "Attendance",
                table: "Employee_Attendance_Exception");

            migrationBuilder.DropColumn(
                name: "AbsenceJustificationId",
                schema: "Attendance",
                table: "Employee_Attendance_Exception");

            migrationBuilder.DropColumn(
                name: "CrossDay",
                schema: "Attendance",
                table: "Attendance_Shift_Override");

            migrationBuilder.DropColumn(
                name: "EndTime",
                schema: "Attendance",
                table: "Attendance_Shift_Override");

            migrationBuilder.DropColumn(
                name: "IsFlexible",
                schema: "Attendance",
                table: "Attendance_Shift_Override");

            migrationBuilder.DropColumn(
                name: "MaxInTime",
                schema: "Attendance",
                table: "Attendance_Shift_Override");

            migrationBuilder.DropColumn(
                name: "MaxOutTime",
                schema: "Attendance",
                table: "Attendance_Shift_Override");

            migrationBuilder.DropColumn(
                name: "MinInTime",
                schema: "Attendance",
                table: "Attendance_Shift_Override");

            migrationBuilder.DropColumn(
                name: "MinOutTime",
                schema: "Attendance",
                table: "Attendance_Shift_Override");

            migrationBuilder.DropColumn(
                name: "NightShift",
                schema: "Attendance",
                table: "Attendance_Shift_Override");

            migrationBuilder.DropColumn(
                name: "RequiredWorkSeconds",
                schema: "Attendance",
                table: "Attendance_Shift_Override");

            migrationBuilder.DropColumn(
                name: "RestEnd",
                schema: "Attendance",
                table: "Attendance_Shift_Override");

            migrationBuilder.DropColumn(
                name: "RestStart",
                schema: "Attendance",
                table: "Attendance_Shift_Override");

            migrationBuilder.DropColumn(
                name: "RoundType",
                schema: "Attendance",
                table: "Attendance_Shift_Override");

            migrationBuilder.DropColumn(
                name: "StartTime",
                schema: "Attendance",
                table: "Attendance_Shift_Override");

            migrationBuilder.DropColumn(
                name: "CrossDay",
                schema: "Attendance",
                table: "Attendance_Shift");

            migrationBuilder.DropColumn(
                name: "EndTime",
                schema: "Attendance",
                table: "Attendance_Shift");

            migrationBuilder.DropColumn(
                name: "IsFlexible",
                schema: "Attendance",
                table: "Attendance_Shift");

            migrationBuilder.DropColumn(
                name: "MaxInTime",
                schema: "Attendance",
                table: "Attendance_Shift");

            migrationBuilder.DropColumn(
                name: "MaxOutTime",
                schema: "Attendance",
                table: "Attendance_Shift");

            migrationBuilder.DropColumn(
                name: "MinInTime",
                schema: "Attendance",
                table: "Attendance_Shift");

            migrationBuilder.DropColumn(
                name: "MinOutTime",
                schema: "Attendance",
                table: "Attendance_Shift");

            migrationBuilder.DropColumn(
                name: "NightShift",
                schema: "Attendance",
                table: "Attendance_Shift");

            migrationBuilder.DropColumn(
                name: "RequiredWorkSeconds",
                schema: "Attendance",
                table: "Attendance_Shift");

            migrationBuilder.DropColumn(
                name: "RestEnd",
                schema: "Attendance",
                table: "Attendance_Shift");

            migrationBuilder.DropColumn(
                name: "RestStart",
                schema: "Attendance",
                table: "Attendance_Shift");

            migrationBuilder.DropColumn(
                name: "RoundType",
                schema: "Attendance",
                table: "Attendance_Shift");

            migrationBuilder.DropColumn(
                name: "StartTime",
                schema: "Attendance",
                table: "Attendance_Shift");

            migrationBuilder.CreateTable(
                name: "Attendance_Shift_Detail",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftId = table.Column<long>(type: "bigint", nullable: false),
                    WeekDay = table.Column<int>(type: "int", nullable: false, comment: "روز هفته (مطابق DayOfWeek)"),
                    IsFlexible = table.Column<bool>(type: "bit", nullable: false, comment: "شیفت منعطف"),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false, comment: "ساعت شروع کار"),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false, comment: "ساعت پایان کار"),
                    RestStart = table.Column<TimeSpan>(type: "time", nullable: true, comment: "شروع استراحت"),
                    RestEnd = table.Column<TimeSpan>(type: "time", nullable: true, comment: "پایان استراحت"),
                    RequiredWorkSeconds = table.Column<int>(type: "int", nullable: false, comment: "مدت کار مورد نیاز (ثانیه)"),
                    NightShift = table.Column<bool>(type: "bit", nullable: false, comment: "شیفت شب"),
                    CrossDay = table.Column<bool>(type: "bit", nullable: false, comment: "عبور از نیمه‌شب"),
                    MinInTime = table.Column<TimeSpan>(type: "time", nullable: true, comment: "حداقل زمان ورود"),
                    MaxInTime = table.Column<TimeSpan>(type: "time", nullable: true, comment: "حداکثر زمان ورود"),
                    MinOutTime = table.Column<TimeSpan>(type: "time", nullable: true, comment: "حداقل زمان خروج"),
                    MaxOutTime = table.Column<TimeSpan>(type: "time", nullable: true, comment: "حداکثر زمان خروج"),
                    RoundType = table.Column<int>(type: "int", nullable: true, comment: "نوع گرد کردن زمان"),
                    title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendance_Shift_Detail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendance_Shift_Detail_Attendance_Shift_ShiftId",
                        column: x => x.ShiftId,
                        principalSchema: "Attendance",
                        principalTable: "Attendance_Shift",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Attendance_Shift_Override_Detail",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftOverrideId = table.Column<long>(type: "bigint", nullable: false),
                    WeekDay = table.Column<int>(type: "int", nullable: false, comment: "روز هفته (مطابق DayOfWeek)"),
                    IsFlexible = table.Column<bool>(type: "bit", nullable: false, comment: "شیفت منعطف"),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false, comment: "ساعت شروع کار"),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false, comment: "ساعت پایان کار"),
                    RestStart = table.Column<TimeSpan>(type: "time", nullable: true, comment: "شروع استراحت"),
                    RestEnd = table.Column<TimeSpan>(type: "time", nullable: true, comment: "پایان استراحت"),
                    RequiredWorkSeconds = table.Column<int>(type: "int", nullable: false, comment: "مدت کار مورد نیاز (ثانیه)"),
                    NightShift = table.Column<bool>(type: "bit", nullable: false, comment: "شیفت شب"),
                    CrossDay = table.Column<bool>(type: "bit", nullable: false, comment: "عبور از نیمه‌شب"),
                    MinInTime = table.Column<TimeSpan>(type: "time", nullable: true, comment: "حداقل زمان ورود"),
                    MaxInTime = table.Column<TimeSpan>(type: "time", nullable: true, comment: "حداکثر زمان ورود"),
                    MinOutTime = table.Column<TimeSpan>(type: "time", nullable: true, comment: "حداقل زمان خروج"),
                    MaxOutTime = table.Column<TimeSpan>(type: "time", nullable: true, comment: "حداکثر زمان خروج"),
                    RoundType = table.Column<int>(type: "int", nullable: true, comment: "نوع گرد کردن زمان"),
                    title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendance_Shift_Override_Detail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendance_Shift_Override_Detail_Attendance_Shift_Override_ShiftOverrideId",
                        column: x => x.ShiftOverrideId,
                        principalSchema: "Attendance",
                        principalTable: "Attendance_Shift_Override",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_Shift_Detail_ShiftId_WeekDay",
                schema: "Attendance",
                table: "Attendance_Shift_Detail",
                columns: new[] { "ShiftId", "WeekDay" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_Shift_Override_Detail_ShiftOverrideId_WeekDay",
                schema: "Attendance",
                table: "Attendance_Shift_Override_Detail",
                columns: new[] { "ShiftOverrideId", "WeekDay" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendance_Shift_Detail",
                schema: "Attendance");

            migrationBuilder.DropTable(
                name: "Attendance_Shift_Override_Detail",
                schema: "Attendance");

            migrationBuilder.AddColumn<long>(
                name: "AbsenceJustificationId",
                schema: "Attendance",
                table: "Employee_Attendance_Exception",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CrossDay",
                schema: "Attendance",
                table: "Attendance_Shift_Override",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "عبور از نیمه‌شب");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                schema: "Attendance",
                table: "Attendance_Shift_Override",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                comment: "ساعت پایان کار");

            migrationBuilder.AddColumn<bool>(
                name: "IsFlexible",
                schema: "Attendance",
                table: "Attendance_Shift_Override",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "شیفت منعطف");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "MaxInTime",
                schema: "Attendance",
                table: "Attendance_Shift_Override",
                type: "time",
                nullable: true,
                comment: "حداکثر زمان ورود");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "MaxOutTime",
                schema: "Attendance",
                table: "Attendance_Shift_Override",
                type: "time",
                nullable: true,
                comment: "حداکثر زمان خروج");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "MinInTime",
                schema: "Attendance",
                table: "Attendance_Shift_Override",
                type: "time",
                nullable: true,
                comment: "حداقل زمان ورود");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "MinOutTime",
                schema: "Attendance",
                table: "Attendance_Shift_Override",
                type: "time",
                nullable: true,
                comment: "حداقل زمان خروج");

            migrationBuilder.AddColumn<bool>(
                name: "NightShift",
                schema: "Attendance",
                table: "Attendance_Shift_Override",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "شیفت شب");

            migrationBuilder.AddColumn<int>(
                name: "RequiredWorkSeconds",
                schema: "Attendance",
                table: "Attendance_Shift_Override",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "مدت کار مورد نیاز (ثانیه)");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "RestEnd",
                schema: "Attendance",
                table: "Attendance_Shift_Override",
                type: "time",
                nullable: true,
                comment: "پایان استراحت");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "RestStart",
                schema: "Attendance",
                table: "Attendance_Shift_Override",
                type: "time",
                nullable: true,
                comment: "شروع استراحت");

            migrationBuilder.AddColumn<int>(
                name: "RoundType",
                schema: "Attendance",
                table: "Attendance_Shift_Override",
                type: "int",
                nullable: true,
                comment: "نوع گرد کردن زمان");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                schema: "Attendance",
                table: "Attendance_Shift_Override",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                comment: "ساعت شروع کار");

            migrationBuilder.AddColumn<bool>(
                name: "CrossDay",
                schema: "Attendance",
                table: "Attendance_Shift",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "عبور از نیمه‌شب");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                schema: "Attendance",
                table: "Attendance_Shift",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                comment: "ساعت پایان کار");

            migrationBuilder.AddColumn<bool>(
                name: "IsFlexible",
                schema: "Attendance",
                table: "Attendance_Shift",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "شیفت منعطف");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "MaxInTime",
                schema: "Attendance",
                table: "Attendance_Shift",
                type: "time",
                nullable: true,
                comment: "حداکثر زمان ورود");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "MaxOutTime",
                schema: "Attendance",
                table: "Attendance_Shift",
                type: "time",
                nullable: true,
                comment: "حداکثر زمان خروج");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "MinInTime",
                schema: "Attendance",
                table: "Attendance_Shift",
                type: "time",
                nullable: true,
                comment: "حداقل زمان ورود");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "MinOutTime",
                schema: "Attendance",
                table: "Attendance_Shift",
                type: "time",
                nullable: true,
                comment: "حداقل زمان خروج");

            migrationBuilder.AddColumn<bool>(
                name: "NightShift",
                schema: "Attendance",
                table: "Attendance_Shift",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "شیفت شب");

            migrationBuilder.AddColumn<int>(
                name: "RequiredWorkSeconds",
                schema: "Attendance",
                table: "Attendance_Shift",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "مدت کار مورد نیاز (ثانیه)");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "RestEnd",
                schema: "Attendance",
                table: "Attendance_Shift",
                type: "time",
                nullable: true,
                comment: "پایان استراحت");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "RestStart",
                schema: "Attendance",
                table: "Attendance_Shift",
                type: "time",
                nullable: true,
                comment: "شروع استراحت");

            migrationBuilder.AddColumn<int>(
                name: "RoundType",
                schema: "Attendance",
                table: "Attendance_Shift",
                type: "int",
                nullable: true,
                comment: "نوع گرد کردن زمان");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                schema: "Attendance",
                table: "Attendance_Shift",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                comment: "ساعت شروع کار");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Attendance_Exception_AbsenceJustificationId",
                schema: "Attendance",
                table: "Employee_Attendance_Exception",
                column: "AbsenceJustificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Attendance_Exception_Attendance_Absence_Type_AbsenceJustificationId",
                schema: "Attendance",
                table: "Employee_Attendance_Exception",
                column: "AbsenceJustificationId",
                principalSchema: "Attendance",
                principalTable: "Attendance_Absence_Type",
                principalColumn: "Id");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Attendance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOttU : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendance_Log",
                schema: "Attendance");

            migrationBuilder.CreateTable(
                name: "Employee_Attendance_Daily_Result",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationChartId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    AttendanceCalendarId = table.Column<long>(type: "bigint", nullable: false),
                    ShiftId = table.Column<long>(type: "bigint", nullable: false),
                    FirstIn = table.Column<DateTime>(type: "datetime", nullable: true, comment: "اولین ورود"),
                    LastOut = table.Column<DateTime>(type: "datetime", nullable: true, comment: "آخرین خروج"),
                    WorkedSeconds = table.Column<int>(type: "int", nullable: false, comment: "ثانیه کارکرد"),
                    RequiredSeconds = table.Column<int>(type: "int", nullable: false, comment: "ثانیه مورد نیاز"),
                    DelaySeconds = table.Column<int>(type: "int", nullable: false, comment: "ثانیه تأخیر"),
                    EarlyLeaveSeconds = table.Column<int>(type: "int", nullable: false, comment: "ثانیه تعجیل در خروج"),
                    AbsentSeconds = table.Column<int>(type: "int", nullable: false, comment: "ثانیه غیبت"),
                    OvertimeSeconds = table.Column<int>(type: "int", nullable: false, comment: "ثانیه اضافه‌کار"),
                    NightWorkSeconds = table.Column<int>(type: "int", nullable: false, comment: "ثانیه کار شب"),
                    HolidayWorkSeconds = table.Column<int>(type: "int", nullable: false, comment: "ثانیه کار در تعطیل"),
                    MissionSeconds = table.Column<int>(type: "int", nullable: false, comment: "ثانیه مأموریت"),
                    LeaveSeconds = table.Column<int>(type: "int", nullable: false, comment: "ثانیه مرخصی"),
                    BreakSeconds = table.Column<int>(type: "int", nullable: false, comment: "ثانیه استراحت"),
                    PaidBreakSeconds = table.Column<int>(type: "int", nullable: false, comment: "ثانیه استراحت با حقوق"),
                    UnpaidBreakSeconds = table.Column<int>(type: "int", nullable: false, comment: "ثانیه استراحت بدون حقوق"),
                    CalculationVersion = table.Column<int>(type: "int", nullable: false, comment: "نسخه محاسبه"),
                    CalculateDate = table.Column<DateTime>(type: "datetime", nullable: true, comment: "تاریخ محاسبه"),
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
                    table.PrimaryKey("PK_Employee_Attendance_Daily_Result", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employee_Attendance_Daily_Result_Attendance_Calendar_AttendanceCalendarId",
                        column: x => x.AttendanceCalendarId,
                        principalSchema: "Attendance",
                        principalTable: "Attendance_Calendar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Attendance_Daily_Result_Attendance_Shift_ShiftId",
                        column: x => x.ShiftId,
                        principalSchema: "Attendance",
                        principalTable: "Attendance_Shift",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Attendance_Daily_Result_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "emp",
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Attendance_Daily_Result_Organisation_Chart_OrganisationChartId",
                        column: x => x.OrganisationChartId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Chart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Employee_Attendance_Log",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    AttendanceDeviceId = table.Column<long>(type: "bigint", nullable: false),
                    DeviceUserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true, comment: "شناسه کاربر در دستگاه"),
                    LogDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "زمان ثبت در دستگاه"),
                    Direction = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true, comment: "جهت تردد"),
                    VerifyMode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true, comment: "نوع احراز هویت"),
                    WorkCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true, comment: "کد کار"),
                    Temperature = table.Column<decimal>(type: "decimal(5,2)", nullable: true, comment: "دمای اندازه‌گیری شده"),
                    Mask = table.Column<bool>(type: "bit", nullable: true, comment: "ماسک"),
                    RawData = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "داده خام دستگاه"),
                    ReceiveDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "زمان دریافت در سامانه"),
                    Status = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true, comment: "وضعیت پردازش"),
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
                    table.PrimaryKey("PK_Employee_Attendance_Log", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employee_Attendance_Log_Attendance_Device_AttendanceDeviceId",
                        column: x => x.AttendanceDeviceId,
                        principalSchema: "Attendance",
                        principalTable: "Attendance_Device",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Attendance_Log_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "emp",
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Attendance_Daily_Result_AttendanceCalendarId",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result",
                column: "AttendanceCalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Attendance_Daily_Result_EmployeeId",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Attendance_Daily_Result_OrganisationChartId_EmployeeId_AttendanceCalendarId",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result",
                columns: new[] { "OrganisationChartId", "EmployeeId", "AttendanceCalendarId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Attendance_Daily_Result_ShiftId",
                schema: "Attendance",
                table: "Employee_Attendance_Daily_Result",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Attendance_Log_AttendanceDeviceId",
                schema: "Attendance",
                table: "Employee_Attendance_Log",
                column: "AttendanceDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Attendance_Log_EmployeeId",
                schema: "Attendance",
                table: "Employee_Attendance_Log",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employee_Attendance_Daily_Result",
                schema: "Attendance");

            migrationBuilder.DropTable(
                name: "Employee_Attendance_Log",
                schema: "Attendance");

            migrationBuilder.CreateTable(
                name: "Attendance_Log",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttendanceDeviceId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeviceUserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true, comment: "شناسه کاربر در دستگاه"),
                    Direction = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true, comment: "جهت تردد"),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LogDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "زمان ثبت در دستگاه"),
                    Mask = table.Column<bool>(type: "bit", nullable: true, comment: "ماسک"),
                    RawData = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "داده خام دستگاه"),
                    ReceiveDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "زمان دریافت در سامانه"),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true, comment: "وضعیت پردازش"),
                    Temperature = table.Column<decimal>(type: "decimal(5,2)", nullable: true, comment: "دمای اندازه‌گیری شده"),
                    VerifyMode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true, comment: "نوع احراز هویت"),
                    WorkCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true, comment: "کد کار"),
                    title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendance_Log", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendance_Log_Attendance_Device_AttendanceDeviceId",
                        column: x => x.AttendanceDeviceId,
                        principalSchema: "Attendance",
                        principalTable: "Attendance_Device",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attendance_Log_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "emp",
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_Log_AttendanceDeviceId",
                schema: "Attendance",
                table: "Attendance_Log",
                column: "AttendanceDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_Log_EmployeeId",
                schema: "Attendance",
                table: "Attendance_Log",
                column: "EmployeeId");
        }
    }
}

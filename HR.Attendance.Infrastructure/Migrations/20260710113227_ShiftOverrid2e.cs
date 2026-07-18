using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Attendance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ShiftOverrid2e : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employee_Attendance_Exception",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationChartId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    AttendanceCalendarId = table.Column<long>(type: "bigint", nullable: false),
                    AbsenceTypeId = table.Column<long>(type: "bigint", nullable: false),
                    ShiftId = table.Column<long>(type: "bigint", nullable: false),
                    StartAt = table.Column<DateTime>(type: "datetime", nullable: false, comment: "لحظه آغاز"),
                    EndAt = table.Column<DateTime>(type: "datetime", nullable: false, comment: "لحظه پایان"),
                    DurationSeconds = table.Column<int>(type: "int", nullable: false, comment: "مدت عدم حضور (ثانیه)"),
                    CalculationVersion = table.Column<int>(type: "int", nullable: false, comment: "نسخه محاسبه"),
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
                    table.PrimaryKey("PK_Employee_Attendance_Exception", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employee_Attendance_Exception_Attendance_Absence_Type_AbsenceTypeId",
                        column: x => x.AbsenceTypeId,
                        principalSchema: "Attendance",
                        principalTable: "Attendance_Absence_Type",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Attendance_Exception_Attendance_Calendar_AttendanceCalendarId",
                        column: x => x.AttendanceCalendarId,
                        principalSchema: "Attendance",
                        principalTable: "Attendance_Calendar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Attendance_Exception_Attendance_Shift_ShiftId",
                        column: x => x.ShiftId,
                        principalSchema: "Attendance",
                        principalTable: "Attendance_Shift",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Attendance_Exception_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "emp",
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Attendance_Exception_Organisation_Chart_OrganisationChartId",
                        column: x => x.OrganisationChartId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Chart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Attendance_Exception_AbsenceTypeId",
                schema: "Attendance",
                table: "Employee_Attendance_Exception",
                column: "AbsenceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Attendance_Exception_AttendanceCalendarId",
                schema: "Attendance",
                table: "Employee_Attendance_Exception",
                column: "AttendanceCalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Attendance_Exception_EmployeeId",
                schema: "Attendance",
                table: "Employee_Attendance_Exception",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Attendance_Exception_OrganisationChartId_EmployeeId_AttendanceCalendarId",
                schema: "Attendance",
                table: "Employee_Attendance_Exception",
                columns: new[] { "OrganisationChartId", "EmployeeId", "AttendanceCalendarId" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Attendance_Exception_ShiftId",
                schema: "Attendance",
                table: "Employee_Attendance_Exception",
                column: "ShiftId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employee_Attendance_Exception",
                schema: "Attendance");
        }
    }
}

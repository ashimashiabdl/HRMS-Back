using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Attendance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Shrfererer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employee_Monthly_Summary",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    OrganisationChartId = table.Column<long>(type: "bigint", nullable: false),
                    CostCenterId = table.Column<long>(type: "bigint", nullable: true),
                    OrganizationUnitId = table.Column<long>(type: "bigint", nullable: true),
                    WorkPlaceId = table.Column<long>(type: "bigint", nullable: true),
                    FunctionDay = table.Column<int>(type: "int", nullable: true, comment: "روزهای ماه"),
                    PersonnelFunctionDay = table.Column<int>(type: "int", nullable: true, comment: "روزهای کارکرد"),
                    PersonnelHourPresent = table.Column<int>(type: "int", nullable: true, comment: "ساعات حضور ساعتی"),
                    PersonnelNoEnter = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PersonnelAbsenceDay = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PersonnelIllnessDay = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PersonnelMissionHours = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PersonnelOverTime = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PersonnelOverTimeMinutes = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PersonnelNightWork = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PersonnelWorkingHolidayHours = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: true, comment: "سال"),
                    Month = table.Column<int>(type: "int", nullable: true, comment: "ماه"),
                    RemoteWorkHours = table.Column<int>(type: "int", nullable: true, comment: "ساعت دورکاری"),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: true, comment: "تأیید شده"),
                    RealFunctionDay = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HolidayFunctionDay = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PersonnelMissionDay = table.Column<int>(type: "int", nullable: true),
                    PaylessDay = table.Column<int>(type: "int", nullable: true),
                    PaylessHour = table.Column<int>(type: "int", nullable: true),
                    ShiftWork10Percent = table.Column<int>(type: "int", nullable: true),
                    ShiftWork15Percent = table.Column<int>(type: "int", nullable: true),
                    ShiftWork22Point5Percent = table.Column<int>(type: "int", nullable: true),
                    DeservedFunctionInHoliday = table.Column<int>(type: "int", nullable: true),
                    DeservedFunctionOutHoliday = table.Column<int>(type: "int", nullable: true),
                    PersonnelNightWorkDay = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PersonnelWorkingHolidaysDay = table.Column<float>(type: "real", nullable: true),
                    LinearFunctionDay = table.Column<long>(type: "bigint", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1012)", maxLength: 1012, nullable: true),
                    ConfirmDate = table.Column<DateTime>(type: "date", nullable: true),
                    PersonnelCeillingOvertime = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PersonnelOverTimeFixed = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AttendanceId = table.Column<long>(type: "bigint", nullable: true),
                    ReceiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ShiftWorkAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: true, comment: "فوق‌العاده نوبت‌کاری"),
                    ShiftCount = table.Column<int>(type: "int", nullable: true, comment: "تعداد شیفت"),
                    PersonnelHourlyWork = table.Column<decimal>(type: "decimal(18,2)", nullable: true, comment: "کارکرد ساعتی (ساعت)"),
                    PersonnelHourlyWorkMinutes = table.Column<decimal>(type: "decimal(18,2)", nullable: true, comment: "کارکرد ساعتی (دقیقه)"),
                    PaylessMinutes = table.Column<int>(type: "int", nullable: true, comment: "دقیقه کسر کار"),
                    PersonnelNightWorkMinutes = table.Column<decimal>(type: "decimal(18,2)", nullable: true, comment: "دقیقه شب‌کاری"),
                    BasijOverTime = table.Column<decimal>(type: "decimal(18,2)", nullable: true, comment: "اضافه‌کاری بسیج"),
                    PersonnelWorkingHolidayMinutes = table.Column<decimal>(type: "decimal(18,2)", nullable: true, comment: "دقیقه تعطیل‌کاری"),
                    OvertimePerCapita = table.Column<decimal>(type: "decimal(18,2)", nullable: true, comment: "سرانه اضافه‌کار"),
                    DisciplinaryOvertime = table.Column<decimal>(type: "decimal(18,2)", nullable: true, comment: "اضافه‌کار انتظامات"),
                    ApprovedOvertimeHours = table.Column<decimal>(type: "decimal(18,2)", nullable: true, comment: "ساعت اضافه‌کار تأییدی"),
                    OvertimeOutsideUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: true, comment: "اضافه‌کار خارج از یگان"),
                    ShiftReplacementOvertime = table.Column<decimal>(type: "decimal(18,2)", nullable: true, comment: "اضافه‌کار جایگزین شیفت"),
                    CashOvertime = table.Column<decimal>(type: "decimal(18,2)", nullable: true, comment: "اضافه‌کار تنخواه"),
                    TotalOvertime = table.Column<decimal>(type: "decimal(18,2)", nullable: true, comment: "مجموع اضافه‌کاری"),
                    MissionAndShift = table.Column<decimal>(type: "decimal(18,2)", nullable: true, comment: "مأموریت و شیفت"),
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
                    table.PrimaryKey("PK_Employee_Monthly_Summary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employee_Monthly_Summary_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "emp",
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Monthly_Summary_Organisation_Chart_CostCenterId",
                        column: x => x.CostCenterId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Chart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Monthly_Summary_Organisation_Chart_OrganisationChartId",
                        column: x => x.OrganisationChartId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Chart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Monthly_Summary_Organisation_Chart_OrganizationUnitId",
                        column: x => x.OrganizationUnitId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Chart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Monthly_Summary_Organisation_Chart_WorkPlaceId",
                        column: x => x.WorkPlaceId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Chart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Monthly_Summary_CostCenterId",
                schema: "Attendance",
                table: "Employee_Monthly_Summary",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Monthly_Summary_EmployeeId",
                schema: "Attendance",
                table: "Employee_Monthly_Summary",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Monthly_Summary_OrganisationChartId_EmployeeId_Year_Month",
                schema: "Attendance",
                table: "Employee_Monthly_Summary",
                columns: new[] { "OrganisationChartId", "EmployeeId", "Year", "Month" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Monthly_Summary_OrganizationUnitId",
                schema: "Attendance",
                table: "Employee_Monthly_Summary",
                column: "OrganizationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Monthly_Summary_WorkPlaceId",
                schema: "Attendance",
                table: "Employee_Monthly_Summary",
                column: "WorkPlaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employee_Monthly_Summary",
                schema: "Attendance");
        }
    }
}

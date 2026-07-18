using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Attendance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ShiftOverride : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attendance_Shift_Override",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationChartId = table.Column<long>(type: "bigint", nullable: false),
                    ShiftId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "توضیحات"),
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
                    table.PrimaryKey("PK_Attendance_Shift_Override", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendance_Shift_Override_Attendance_Shift_ShiftId",
                        column: x => x.ShiftId,
                        principalSchema: "Attendance",
                        principalTable: "Attendance_Shift",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attendance_Shift_Override_Organisation_Chart_OrganisationChartId",
                        column: x => x.OrganisationChartId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Chart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_Shift_Override_OrganisationChartId_ShiftId_StartDate_EndDate",
                schema: "Attendance",
                table: "Attendance_Shift_Override",
                columns: new[] { "OrganisationChartId", "ShiftId", "StartDate", "EndDate" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_Shift_Override_ShiftId",
                schema: "Attendance",
                table: "Attendance_Shift_Override",
                column: "ShiftId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendance_Shift_Override",
                schema: "Attendance");
        }
    }
}

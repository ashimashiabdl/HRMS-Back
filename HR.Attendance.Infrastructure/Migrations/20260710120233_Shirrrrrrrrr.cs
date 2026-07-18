using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Attendance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Shirrrrrrrrr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AbsenceJustificationId",
                schema: "Attendance",
                table: "Employee_Attendance_Exception",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Employee_Exception_Justification_Request_State",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StateCode = table.Column<int>(type: "int", nullable: false, comment: "کد وضعیت"),
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
                    table.PrimaryKey("PK_Employee_Exception_Justification_Request_State", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employee_Exception_Justification_Request",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeAttendanceExceptionId = table.Column<long>(type: "bigint", nullable: false),
                    AbsenceTypeId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeExceptionJustificationRequestStateId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true, comment: "توضیحات"),
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
                    table.PrimaryKey("PK_Employee_Exception_Justification_Request", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employee_Exception_Justification_Request_Attendance_Absence_Type_AbsenceTypeId",
                        column: x => x.AbsenceTypeId,
                        principalSchema: "Attendance",
                        principalTable: "Attendance_Absence_Type",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Exception_Justification_Request_Employee_Attendance_Exception_EmployeeAttendanceExceptionId",
                        column: x => x.EmployeeAttendanceExceptionId,
                        principalSchema: "Attendance",
                        principalTable: "Employee_Attendance_Exception",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Exception_Justification_Request_Employee_Exception_Justification_Request_State_EmployeeExceptionJustificationReques~",
                        column: x => x.EmployeeExceptionJustificationRequestStateId,
                        principalSchema: "Attendance",
                        principalTable: "Employee_Exception_Justification_Request_State",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Attendance_Exception_AbsenceJustificationId",
                schema: "Attendance",
                table: "Employee_Attendance_Exception",
                column: "AbsenceJustificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Exception_Justification_Request_AbsenceTypeId",
                schema: "Attendance",
                table: "Employee_Exception_Justification_Request",
                column: "AbsenceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Exception_Justification_Request_EmployeeAttendanceExceptionId",
                schema: "Attendance",
                table: "Employee_Exception_Justification_Request",
                column: "EmployeeAttendanceExceptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Exception_Justification_Request_EmployeeExceptionJustificationRequestStateId",
                schema: "Attendance",
                table: "Employee_Exception_Justification_Request",
                column: "EmployeeExceptionJustificationRequestStateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Attendance_Exception_Attendance_Absence_Type_AbsenceJustificationId",
                schema: "Attendance",
                table: "Employee_Attendance_Exception",
                column: "AbsenceJustificationId",
                principalSchema: "Attendance",
                principalTable: "Attendance_Absence_Type",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Attendance_Exception_Attendance_Absence_Type_AbsenceJustificationId",
                schema: "Attendance",
                table: "Employee_Attendance_Exception");

            migrationBuilder.DropTable(
                name: "Employee_Exception_Justification_Request",
                schema: "Attendance");

            migrationBuilder.DropTable(
                name: "Employee_Exception_Justification_Request_State",
                schema: "Attendance");

            migrationBuilder.DropIndex(
                name: "IX_Employee_Attendance_Exception_AbsenceJustificationId",
                schema: "Attendance",
                table: "Employee_Attendance_Exception");

            migrationBuilder.DropColumn(
                name: "AbsenceJustificationId",
                schema: "Attendance",
                table: "Employee_Attendance_Exception");
        }
    }
}

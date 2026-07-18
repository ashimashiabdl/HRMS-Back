using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Attendance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeShiftAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

   

            migrationBuilder.CreateTable(
                name: "Attendance_Employee_Shift_Assignment",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationChartId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    ShiftId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "توضیحات"),
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
                    table.PrimaryKey("PK_Attendance_Employee_Shift_Assignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendance_Employee_Shift_Assignment_Attendance_Shift_ShiftId",
                        column: x => x.ShiftId,
                        principalSchema: "Attendance",
                        principalTable: "Attendance_Shift",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attendance_Employee_Shift_Assignment_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "emp",
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attendance_Employee_Shift_Assignment_Organisation_Chart_OrganisationChartId",
                        column: x => x.OrganisationChartId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Chart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

  
            migrationBuilder.CreateIndex(
                name: "IX_Attendance_Employee_Shift_Assignment_EmployeeId",
                schema: "Attendance",
                table: "Attendance_Employee_Shift_Assignment",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_Employee_Shift_Assignment_OrganisationChartId",
                schema: "Attendance",
                table: "Attendance_Employee_Shift_Assignment",
                column: "OrganisationChartId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_Employee_Shift_Assignment_ShiftId",
                schema: "Attendance",
                table: "Attendance_Employee_Shift_Assignment",
                column: "ShiftId");

   
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
         

            migrationBuilder.DropTable(
                name: "Attendance_Employee_Shift_Assignment",
                schema: "Attendance");

       
        }
    }
}

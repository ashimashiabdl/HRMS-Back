using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Attendance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Shrfvvrrrr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LeaveTypeId",
                schema: "Attendance",
                table: "Employee_Exception_Justification_Request",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Exception_Justification_Request_LeaveTypeId",
                schema: "Attendance",
                table: "Employee_Exception_Justification_Request",
                column: "LeaveTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Exception_Justification_Request_LeaveType_LeaveTypeId",
                schema: "Attendance",
                table: "Employee_Exception_Justification_Request",
                column: "LeaveTypeId",
                principalSchema: "bas",
                principalTable: "LeaveType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Exception_Justification_Request_LeaveType_LeaveTypeId",
                schema: "Attendance",
                table: "Employee_Exception_Justification_Request");

            migrationBuilder.DropIndex(
                name: "IX_Employee_Exception_Justification_Request_LeaveTypeId",
                schema: "Attendance",
                table: "Employee_Exception_Justification_Request");

            migrationBuilder.DropColumn(
                name: "LeaveTypeId",
                schema: "Attendance",
                table: "Employee_Exception_Justification_Request");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Employee.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Awewuhhts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CitizenshipId",
                schema: "emp",
                table: "Employee",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_CitizenshipId",
                schema: "emp",
                table: "Employee",
                column: "CitizenshipId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Base_Table_Value_CitizenshipId",
                schema: "emp",
                table: "Employee",
                column: "CitizenshipId",
                principalSchema: "bas",
                principalTable: "Base_Table_Value",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Base_Table_Value_CitizenshipId",
                schema: "emp",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_CitizenshipId",
                schema: "emp",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "CitizenshipId",
                schema: "emp",
                table: "Employee");
        }
    }
}

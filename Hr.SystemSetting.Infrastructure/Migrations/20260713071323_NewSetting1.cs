using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hr.SystemSetting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewSetting1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EnterTypeId",
                schema: "Setting",
                table: "Organisation_EmployeeType_Settlement_Item",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "FixValue",
                schema: "Setting",
                table: "Organisation_EmployeeType_Settlement_Item",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_EmployeeType_Settlement_Item_EnterTypeId",
                schema: "Setting",
                table: "Organisation_EmployeeType_Settlement_Item",
                column: "EnterTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organisation_EmployeeType_Settlement_Item_Base_Table_Value_EnterTypeId",
                schema: "Setting",
                table: "Organisation_EmployeeType_Settlement_Item",
                column: "EnterTypeId",
                principalSchema: "bas",
                principalTable: "Base_Table_Value",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organisation_EmployeeType_Settlement_Item_Base_Table_Value_EnterTypeId",
                schema: "Setting",
                table: "Organisation_EmployeeType_Settlement_Item");

            migrationBuilder.DropIndex(
                name: "IX_Organisation_EmployeeType_Settlement_Item_EnterTypeId",
                schema: "Setting",
                table: "Organisation_EmployeeType_Settlement_Item");

            migrationBuilder.DropColumn(
                name: "EnterTypeId",
                schema: "Setting",
                table: "Organisation_EmployeeType_Settlement_Item");

            migrationBuilder.DropColumn(
                name: "FixValue",
                schema: "Setting",
                table: "Organisation_EmployeeType_Settlement_Item");
        }
    }
}

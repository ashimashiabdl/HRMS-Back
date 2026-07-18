using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Order.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class dddqqqq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AddColumn<bool>(
                name: "DatesFromWageExcel",
                schema: "Order",
                table: "Batch_Request",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseMappedExcelColumns",
                schema: "Order",
                table: "Batch_Request",
                type: "bit",
                nullable: false,
                defaultValue: false);

          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Node_WorkFlow_WorkFlowId",
                schema: "wf",
                table: "Node");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkFlow_Instance_Interdict_Order_InterdictOrderId",
                schema: "wf",
                table: "WorkFlow_Instance");

            migrationBuilder.DropTable(
                name: "Node_Role_Rel",
                schema: "wf");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "Identity");

            migrationBuilder.DropIndex(
                name: "IX_Node_WorkFlowId",
                schema: "wf",
                table: "Node");

            migrationBuilder.DropColumn(
                name: "EmployeeSettlementId",
                schema: "wf",
                table: "WorkFlow_Instance");

            migrationBuilder.DropColumn(
                name: "WorkFlowId",
                schema: "wf",
                table: "Node");

            migrationBuilder.DropColumn(
                name: "DatesFromWageExcel",
                schema: "Order",
                table: "Batch_Request");

            migrationBuilder.DropColumn(
                name: "UseMappedExcelColumns",
                schema: "Order",
                table: "Batch_Request");

            migrationBuilder.AlterColumn<long>(
                name: "InterdictOrderId",
                schema: "wf",
                table: "WorkFlow_Instance",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkFlow_Instance_Interdict_Order_InterdictOrderId",
                schema: "wf",
                table: "WorkFlow_Instance",
                column: "InterdictOrderId",
                principalSchema: "Order",
                principalTable: "Interdict_Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

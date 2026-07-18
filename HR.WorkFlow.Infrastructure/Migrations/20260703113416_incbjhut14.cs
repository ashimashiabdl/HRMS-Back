using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.WorkFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class incbjhut14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkFlow_Instance_Interdict_Order_InterdictOrderId",
                schema: "wf",
                table: "WorkFlow_Instance");

            migrationBuilder.AlterColumn<long>(
                name: "InterdictOrderId",
                schema: "wf",
                table: "WorkFlow_Instance",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "EmployeeSettlementId",
                schema: "wf",
                table: "WorkFlow_Instance",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkFlow_Instance_Interdict_Order_InterdictOrderId",
                schema: "wf",
                table: "WorkFlow_Instance",
                column: "InterdictOrderId",
                principalSchema: "Order",
                principalTable: "Interdict_Order",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkFlow_Instance_Interdict_Order_InterdictOrderId",
                schema: "wf",
                table: "WorkFlow_Instance");

            migrationBuilder.DropColumn(
                name: "EmployeeSettlementId",
                schema: "wf",
                table: "WorkFlow_Instance");

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

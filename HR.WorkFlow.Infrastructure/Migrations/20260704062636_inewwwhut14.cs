using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.WorkFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class inewwwhut14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
       

            migrationBuilder.AddForeignKey(
                name: "FK_Node_WorkFlow_WorkFlowId",
                schema: "wf",
                table: "Node",
                column: "WorkFlowId",
                principalSchema: "wf",
                principalTable: "WorkFlow",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Node_WorkFlow_WorkFlowId",
                schema: "wf",
                table: "Node");

            migrationBuilder.DropIndex(
                name: "IX_Node_WorkFlowId",
                schema: "wf",
                table: "Node");

            migrationBuilder.DropColumn(
                name: "WorkFlowId",
                schema: "wf",
                table: "Node");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Organisation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Accoin74 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PositionManagementLevelId",
                schema: "Org",
                table: "Organisation_Position",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PositionStateId",
                schema: "Org",
                table: "Organisation_Position",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

         

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Position_PositionManagementLevelId",
                schema: "Org",
                table: "Organisation_Position",
                column: "PositionManagementLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Position_PositionStateId",
                schema: "Org",
                table: "Organisation_Position",
                column: "PositionStateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organisation_Position_Position_Management_Level_PositionManagementLevelId",
                schema: "Org",
                table: "Organisation_Position",
                column: "PositionManagementLevelId",
                principalSchema: "bas",
                principalTable: "Position_Management_Level",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Organisation_Position_Position_State_PositionStateId",
                schema: "Org",
                table: "Organisation_Position",
                column: "PositionStateId",
                principalSchema: "bas",
                principalTable: "Position_State",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organisation_Position_Position_Management_Level_PositionManagementLevelId",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropForeignKey(
                name: "FK_Organisation_Position_Position_State_PositionStateId",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropTable(
                name: "Position_Management_Level",
                schema: "bas");

            migrationBuilder.DropTable(
                name: "Position_State",
                schema: "bas");

            migrationBuilder.DropIndex(
                name: "IX_Organisation_Position_PositionManagementLevelId",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropIndex(
                name: "IX_Organisation_Position_PositionStateId",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropColumn(
                name: "PositionManagementLevelId",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropColumn(
                name: "PositionStateId",
                schema: "Org",
                table: "Organisation_Position");
        }
    }
}

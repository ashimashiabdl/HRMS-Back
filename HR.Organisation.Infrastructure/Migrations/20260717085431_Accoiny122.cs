using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Organisation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Accoiny122 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RankId",
                schema: "Org",
                table: "Organisation_Position",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "title",
                schema: "Org",
                table: "Organisation_Position",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "PlaceId",
                schema: "Org",
                table: "Organisation_Chart",
                type: "bigint",
                nullable: true);

            
            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Position_RankId",
                schema: "Org",
                table: "Organisation_Position",
                column: "RankId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Chart_PlaceId",
                schema: "Org",
                table: "Organisation_Chart",
                column: "PlaceId");

        

            migrationBuilder.AddForeignKey(
                name: "FK_Organisation_Chart_Places_PlaceId",
                schema: "Org",
                table: "Organisation_Chart",
                column: "PlaceId",
                principalSchema: "bas",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Organisation_Position_Rank_RankId",
                schema: "Org",
                table: "Organisation_Position",
                column: "RankId",
                principalSchema: "bas",
                principalTable: "Rank",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organisation_Chart_Places_PlaceId",
                schema: "Org",
                table: "Organisation_Chart");

            migrationBuilder.DropForeignKey(
                name: "FK_Organisation_Position_Rank_RankId",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropTable(
                name: "Places",
                schema: "bas");

            migrationBuilder.DropTable(
                name: "Rank",
                schema: "bas");

            migrationBuilder.DropIndex(
                name: "IX_Organisation_Position_RankId",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropIndex(
                name: "IX_Organisation_Chart_PlaceId",
                schema: "Org",
                table: "Organisation_Chart");

            migrationBuilder.DropColumn(
                name: "RankId",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropColumn(
                name: "title",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropColumn(
                name: "PlaceId",
                schema: "Org",
                table: "Organisation_Chart");
        }
    }
}

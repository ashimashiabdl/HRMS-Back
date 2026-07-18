using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Identity.Infrastructure.Migrations.CustomIdentity
{
    /// <inheritdoc />
    public partial class Addxcsdsdxwwwcdf11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Salt",
                schema: "Identity",
                table: "User_Password_History",
                type: "varbinary(64)",
                maxLength: 64,
                nullable: true);

          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_ConfidentialityLevel_ConfidentialityLevelId",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ConfidentialityLevel",
                schema: "bas");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ConfidentialityLevelId",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Salt",
                schema: "Identity",
                table: "User_Password_History");

            migrationBuilder.DropColumn(
                name: "ConfidentialityLevelId",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DeactivationReason",
                schema: "Identity",
                table: "AspNetUsers");
        }
    }
}

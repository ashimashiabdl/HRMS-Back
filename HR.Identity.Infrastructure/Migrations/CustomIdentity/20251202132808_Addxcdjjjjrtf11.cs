using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Identity.Infrastructure.Migrations.CustomIdentity
{
    /// <inheritdoc />
    public partial class Addxcdjjjjrtf11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_User_Default_Setting_DefaultOrganId",
                schema: "Identity",
                table: "User_Default_Setting",
                column: "DefaultOrganId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Default_Setting_Organisation_Chart_DefaultOrganId",
                schema: "Identity",
                table: "User_Default_Setting",
                column: "DefaultOrganId",
                principalSchema: "Org",
                principalTable: "Organisation_Chart",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Default_Setting_Organisation_Chart_DefaultOrganId",
                schema: "Identity",
                table: "User_Default_Setting");

            migrationBuilder.DropIndex(
                name: "IX_User_Default_Setting_DefaultOrganId",
                schema: "Identity",
                table: "User_Default_Setting");
        }
    }
}

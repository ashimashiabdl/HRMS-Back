using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.FormulaEngine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Setgd444fdfero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
       
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IPAddress",
                schema: "For",
                table: "Formula_Definition_History",
                newName: "IpAddress");

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                schema: "For",
                table: "Formula_Definition_History",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<string>(
                name: "IPAddress",
                schema: "For",
                table: "Formula_Definition_History",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }
    }
}

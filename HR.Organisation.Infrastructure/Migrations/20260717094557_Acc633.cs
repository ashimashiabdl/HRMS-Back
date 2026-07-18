using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Organisation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Acc633 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxTenureYears",
                schema: "Org",
                table: "Organisation_Position",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Other1",
                schema: "Org",
                table: "Organisation_Position",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Other2",
                schema: "Org",
                table: "Organisation_Position",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                schema: "Org",
                table: "Organisation_Position",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UniqueIdentifier",
                schema: "Org",
                table: "Organisation_Position",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxTenureYears",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropColumn(
                name: "Other1",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropColumn(
                name: "Other2",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropColumn(
                name: "ShortName",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropColumn(
                name: "UniqueIdentifier",
                schema: "Org",
                table: "Organisation_Position");
        }
    }
}

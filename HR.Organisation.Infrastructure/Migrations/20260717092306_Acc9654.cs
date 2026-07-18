using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Organisation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Acc9654 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AbbreviationMark",
                schema: "Org",
                table: "Organisation_Chart",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "Org",
                table: "Organisation_Chart",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                schema: "Org",
                table: "Organisation_Chart",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UniqueIdentifier",
                schema: "Org",
                table: "Organisation_Chart",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AbbreviationMark",
                schema: "Org",
                table: "Organisation_Chart");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "Org",
                table: "Organisation_Chart");

            migrationBuilder.DropColumn(
                name: "ShortName",
                schema: "Org",
                table: "Organisation_Chart");

            migrationBuilder.DropColumn(
                name: "UniqueIdentifier",
                schema: "Org",
                table: "Organisation_Chart");
        }
    }
}

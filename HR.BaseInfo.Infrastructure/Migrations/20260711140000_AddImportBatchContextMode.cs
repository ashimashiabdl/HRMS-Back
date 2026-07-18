using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.BaseInfo.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImportBatchContextMode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContextMode",
                schema: "bas",
                table: "Import_Batch",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContextMode",
                schema: "bas",
                table: "Import_Batch");
        }
    }
}

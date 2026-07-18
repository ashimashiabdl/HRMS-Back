using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Employee.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Engl5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsComputable",
                schema: "emp",
                table: "Insurance",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsComputable",
                schema: "emp",
                table: "Insurance");
        }
    }
}

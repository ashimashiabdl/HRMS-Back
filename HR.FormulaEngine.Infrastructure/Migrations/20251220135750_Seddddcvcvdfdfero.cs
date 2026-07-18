using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.FormulaEngine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Seddddcvcvdfdfero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "For",
                table: "Formula_Table_Value",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "For",
                table: "Formula_Table_Value",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "For",
                table: "Formula_Table",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "For",
                table: "Formula_Table",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "For",
                table: "Formula_Operand",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "For",
                table: "Formula_Operand",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "For",
                table: "Formula_Definition_History",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "For",
                table: "Formula_Definition_History",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "For",
                table: "Formula_Definition",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "For",
                table: "Formula_Definition",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "For",
                table: "Formula_Database_Function_Definition",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "For",
                table: "Formula_Database_Function_Definition",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "bas",
                table: "Organization_Type");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "bas",
                table: "Organization_Type");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Setting",
                table: "Organisation_Formula");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Setting",
                table: "Organisation_Formula");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Org",
                table: "Organisation_Chart");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Org",
                table: "Organisation_Chart");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "For",
                table: "Formula_Table_Value");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "For",
                table: "Formula_Table_Value");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "For",
                table: "Formula_Table");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "For",
                table: "Formula_Table");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "For",
                table: "Formula_Operand");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "For",
                table: "Formula_Operand");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "For",
                table: "Formula_Definition_History");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "For",
                table: "Formula_Definition_History");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "For",
                table: "Formula_Definition");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "For",
                table: "Formula_Definition");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "For",
                table: "Formula_Database_Function_Definition");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "For",
                table: "Formula_Database_Function_Definition");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "bas",
                table: "Formula");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "bas",
                table: "Formula");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "bas",
                table: "Base_Table_Value");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "bas",
                table: "Base_Table_Value");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "bas",
                table: "Base_Table");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "bas",
                table: "Base_Table");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Report.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Mweuusdsdsdkjjkvesds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "rpt",
                table: "Reportable_Field",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "rpt",
                table: "Reportable_Field",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "rpt",
                table: "Reportable_Entity",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "rpt",
                table: "Reportable_Entity",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "rpt",
                table: "PayLocation_Progress_Report",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "rpt",
                table: "PayLocation_Progress_Report",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

        

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "rpt",
                table: "Field_Operator",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "rpt",
                table: "Field_Operator",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "rpt",
                table: "Field_DataType",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "rpt",
                table: "Field_DataType",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "rpt",
                table: "Employee_Property",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "rpt",
                table: "Employee_Property",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "rpt",
                table: "Dynamic_Report_Parameter",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "rpt",
                table: "Dynamic_Report_Parameter",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "rpt",
                table: "Dynamic_Report",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "rpt",
                table: "Dynamic_Report",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

         
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "rpt",
                table: "Reportable_Field");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "rpt",
                table: "Reportable_Field");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "rpt",
                table: "Reportable_Entity");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "rpt",
                table: "Reportable_Entity");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "rpt",
                table: "PayLocation_Progress_Report");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "rpt",
                table: "PayLocation_Progress_Report");

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
                table: "Organisation_MRT");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Setting",
                table: "Organisation_MRT");

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
                schema: "rpt",
                table: "Field_Operator");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "rpt",
                table: "Field_Operator");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "rpt",
                table: "Field_DataType");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "rpt",
                table: "Field_DataType");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "rpt",
                table: "Employee_Property");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "rpt",
                table: "Employee_Property");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "rpt",
                table: "Dynamic_Report_Parameter");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "rpt",
                table: "Dynamic_Report_Parameter");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "rpt",
                table: "Dynamic_Report");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "rpt",
                table: "Dynamic_Report");

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

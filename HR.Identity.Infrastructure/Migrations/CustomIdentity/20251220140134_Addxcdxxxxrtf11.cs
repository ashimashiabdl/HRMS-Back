using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Identity.Infrastructure.Migrations.CustomIdentity
{
    /// <inheritdoc />
    public partial class Addxcdxxxxrtf11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Identity",
                table: "User_WorkPlace",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "User_WorkPlace",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Identity",
                table: "User_PayLocation",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "User_PayLocation",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Identity",
                table: "User_Password_History",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "User_Password_History",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Identity",
                table: "User_OrganizationUnit",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "User_OrganizationUnit",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Identity",
                table: "User_Login_History",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "User_Login_History",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Identity",
                table: "User_Default_Setting",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "User_Default_Setting",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Identity",
                table: "User_CostCenter",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "User_CostCenter",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Identity",
                table: "User_WorkPlace");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "User_WorkPlace");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Identity",
                table: "User_PayLocation");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "User_PayLocation");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Identity",
                table: "User_Password_History");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "User_Password_History");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Identity",
                table: "User_OrganizationUnit");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "User_OrganizationUnit");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Identity",
                table: "User_Login_History");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "User_Login_History");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Identity",
                table: "User_Default_Setting");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "User_Default_Setting");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Identity",
                table: "User_CostCenter");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "User_CostCenter");

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
                schema: "Org",
                table: "Organisation_Chart");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Org",
                table: "Organisation_Chart");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "bas",
                table: "ConfidentialityLevel");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "bas",
                table: "ConfidentialityLevel");
        }
    }
}

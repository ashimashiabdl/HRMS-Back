using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleAuiweyriwuerynnnsol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Identity",
                table: "User_Reportable_Entity",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "User_Reportable_Entity",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Identity",
                table: "User_Report",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "User_Report",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Identity",
                table: "User_Menu",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "User_Menu",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Identity",
                table: "Role_Reportable_Entity",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "Role_Reportable_Entity",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Identity",
                table: "Role_Report",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "Role_Report",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Identity",
                table: "Role_Menu",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "Role_Menu",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Identity",
                table: "Permission_Route",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "Permission_Route",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

 


            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Identity",
                table: "MessageAttachment",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "MessageAttachment",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Identity",
                table: "Message",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "Message",
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
                table: "User_Reportable_Entity");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "User_Reportable_Entity");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Identity",
                table: "User_Report");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "User_Report");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Identity",
                table: "User_Menu");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "User_Menu");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Identity",
                table: "Role_Reportable_Entity");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "Role_Reportable_Entity");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Identity",
                table: "Role_Report");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "Role_Report");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Identity",
                table: "Role_Menu");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "Role_Menu");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Identity",
                table: "Permission_Route");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "Permission_Route");

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
                schema: "Identity",
                table: "MessageAttachment");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "MessageAttachment");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Identity",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "Message");

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

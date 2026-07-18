using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Employee.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeColumnDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "TwoFactorEnabled",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "SubsystemId",
                schema: "emp",
                table: "Employee",
                type: "int",
                nullable: false,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "SecurityStamp",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(max)",
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SectId",
                schema: "emp",
                table: "Employee",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReleaseReason",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PrivateJobStatus",
                schema: "emp",
                table: "Employee",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "PhoneNumberConfirmed",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PersonelCode",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(max)",
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PassportNo",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NationalNo",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MartyrChildTrackingCode",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LostIssueSerialString",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IssueSerialString",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IssueSerialOrder",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IssueSerialChar",
                schema: "emp",
                table: "Employee",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsWomenHead",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsWelfareBenefits",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsVerify",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsRetired",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsHekmat",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCashBenefits",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAdmin",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "InOutCard",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Imperfective",
                schema: "emp",
                table: "Employee",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdentityNo",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FatherName",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "EmailConfirmed",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "Disabled",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Descriptions",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(max)",
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ActiveName",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountingSystemEmployeeId",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccessFailedCount",
                schema: "emp",
                table: "Employee",
                type: "int",
                nullable: false,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "TwoFactorEnabled",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<int>(
                name: "SubsystemId",
                schema: "emp",
                table: "Employee",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "SecurityStamp",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "SectId",
                schema: "emp",
                table: "Employee",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "ReleaseReason",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "PrivateJobStatus",
                schema: "emp",
                table: "Employee",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<bool>(
                name: "PhoneNumberConfirmed",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "PersonelCode",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "PassportNo",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "NationalNo",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "MartyrChildTrackingCode",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LostIssueSerialString",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "IssueSerialString",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "IssueSerialOrder",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "IssueSerialChar",
                schema: "emp",
                table: "Employee",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsWomenHead",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsWelfareBenefits",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsVerify",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsRetired",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsHekmat",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCashBenefits",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAdmin",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "InOutCard",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "Imperfective",
                schema: "emp",
                table: "Employee",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityNo",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "FatherName",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "EmailConfirmed",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "Disabled",
                schema: "emp",
                table: "Employee",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "Descriptions",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "ActiveName",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "AccountingSystemEmployeeId",
                schema: "emp",
                table: "Employee",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "AccessFailedCount",
                schema: "emp",
                table: "Employee",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "(0)");
        }
    }
}

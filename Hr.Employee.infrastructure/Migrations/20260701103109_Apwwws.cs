using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Employee.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Apwwws : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "title",
                schema: "emp",
                table: "Attendance");

            migrationBuilder.DropColumn(
                name: "title",
                schema: "emp",
                table: "Absence_Record");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                schema: "emp",
                table: "Work",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<int>(
                name: "YearMult",
                schema: "emp",
                table: "Work",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WorkPlaceDesc",
                schema: "emp",
                table: "Work",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RetiredMult",
                schema: "emp",
                table: "Work",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LetterNumber",
                schema: "emp",
                table: "Work",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastTitle",
                schema: "emp",
                table: "Work",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Work",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Work",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsComputeable",
                schema: "emp",
                table: "Work",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "InsHsyYear",
                schema: "emp",
                table: "Work",
                type: "tinyint",
                nullable: false,
                defaultValueSql: "(0)",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<byte>(
                name: "InsHsyMonth",
                schema: "emp",
                table: "Work",
                type: "tinyint",
                nullable: false,
                defaultValueSql: "(0)",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<byte>(
                name: "InsHsyDay",
                schema: "emp",
                table: "Work",
                type: "tinyint",
                nullable: false,
                defaultValueSql: "(0)",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Work",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ExperienceMult",
                schema: "emp",
                table: "Work",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Work",
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
                table: "Work",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "AcptInsHsyYear",
                schema: "emp",
                table: "Work",
                type: "tinyint",
                nullable: false,
                defaultValueSql: "(0)",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<byte>(
                name: "AcptInsHsyMonth",
                schema: "emp",
                table: "Work",
                type: "tinyint",
                nullable: false,
                defaultValueSql: "(0)",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<byte>(
                name: "AcptInsHsyDay",
                schema: "emp",
                table: "Work",
                type: "tinyint",
                nullable: false,
                defaultValueSql: "(0)",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<string>(
                name: "TrackingCode",
                schema: "emp",
                table: "War",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "PercentAnnualIncrease",
                schema: "emp",
                table: "War",
                type: "float",
                nullable: true,
                defaultValueSql: "((0))",
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LetterNumber",
                schema: "emp",
                table: "War",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "War",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JebheOperations",
                schema: "emp",
                table: "War",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "War",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsContinues",
                schema: "emp",
                table: "War",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "emp",
                table: "War",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "War",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DurationYear",
                schema: "emp",
                table: "War",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DurationMonth",
                schema: "emp",
                table: "War",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DurationDay",
                schema: "emp",
                table: "War",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descriptions",
                schema: "emp",
                table: "War",
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
                table: "War",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AcceptableDurationForTaxExemption",
                schema: "emp",
                table: "War",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Value",
                schema: "emp",
                table: "Temp_Punishment_Encourage",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UnitValue",
                schema: "emp",
                table: "Temp_Punishment_Encourage",
                type: "int",
                nullable: false,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "NationalNo",
                schema: "emp",
                table: "Temp_Punishment_Encourage",
                type: "nvarchar(max)",
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Temp_Punishment_Encourage",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Temp_Punishment_Encourage",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Temp_Punishment_Encourage",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Temp_Punishment_Encourage",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Value",
                schema: "emp",
                table: "Punishment_Encourage",
                type: "int",
                nullable: false,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UnitValue",
                schema: "emp",
                table: "Punishment_Encourage",
                type: "int",
                nullable: false,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Punishment_Encourage",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsGroup",
                schema: "emp",
                table: "Punishment_Encourage",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Punishment_Encourage",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Punishment_Encourage",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Punishment_Encourage",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 4096,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Punishment_Encourage",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrackingCode",
                schema: "emp",
                table: "Other_Veteran",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SacrificePercent",
                schema: "emp",
                table: "Other_Veteran",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LetterNumber",
                schema: "emp",
                table: "Other_Veteran",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Other_Veteran",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsLast",
                schema: "emp",
                table: "Other_Veteran",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Other_Veteran",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsComputeable",
                schema: "emp",
                table: "Other_Veteran",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "emp",
                table: "Other_Veteran",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Other_Veteran",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DurationYear",
                schema: "emp",
                table: "Other_Veteran",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DurationMonth",
                schema: "emp",
                table: "Other_Veteran",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DurationDay",
                schema: "emp",
                table: "Other_Veteran",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descriptions",
                schema: "emp",
                table: "Other_Veteran",
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
                table: "Other_Veteran",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameOfPeriod",
                schema: "emp",
                table: "Military_Service",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MilitaryMinDuration",
                schema: "emp",
                table: "Military_Service",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MilitaryFullDuration",
                schema: "emp",
                table: "Military_Service",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "MilitaryDuration",
                schema: "emp",
                table: "Military_Service",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MilitariSerialNo",
                schema: "emp",
                table: "Military_Service",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Military_Service",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsLast",
                schema: "emp",
                table: "Military_Service",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Military_Service",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsContinue",
                schema: "emp",
                table: "Military_Service",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsComputable",
                schema: "emp",
                table: "Military_Service",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Military_Service",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descriptions",
                schema: "emp",
                table: "Military_Service",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Military_Service",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConfirmedLetterNo",
                schema: "emp",
                table: "Military_Service",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrackingCode",
                schema: "emp",
                table: "Isar",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LetterNumber",
                schema: "emp",
                table: "Isar",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Isar",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Isarpercent",
                schema: "emp",
                table: "Isar",
                type: "real",
                nullable: true,
                defaultValueSql: "((0))",
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IsarInjuerdOrgan",
                schema: "emp",
                table: "Isar",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IsarInability",
                schema: "emp",
                table: "Isar",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IsarDurationYear",
                schema: "emp",
                table: "Isar",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IsarDurationMonth",
                schema: "emp",
                table: "Isar",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IsarDurationDay",
                schema: "emp",
                table: "Isar",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Isar",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsContinues",
                schema: "emp",
                table: "Isar",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "emp",
                table: "Isar",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Isar",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Isar",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Isar",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                schema: "emp",
                table: "Insurance_Detail",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Month",
                schema: "emp",
                table: "Insurance_Detail",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Insurance_Detail",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsOptionalInsurnce",
                schema: "emp",
                table: "Insurance_Detail",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsFullInsurnce",
                schema: "emp",
                table: "Insurance_Detail",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Insurance_Detail",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsComputable",
                schema: "emp",
                table: "Insurance_Detail",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Insurance_Detail",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Desc",
                schema: "emp",
                table: "Insurance_Detail",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Insurance_Detail",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccDay",
                schema: "emp",
                table: "Insurance_Detail",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Insurance",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsLast",
                schema: "emp",
                table: "Insurance",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Insurance",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "InsuranceNumber",
                schema: "emp",
                table: "Insurance",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32);

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Insurance",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "HasSupplementaryInsurance",
                schema: "emp",
                table: "Insurance",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Insurance",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccDay",
                schema: "emp",
                table: "Insurance",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Image",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Image",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDefault",
                schema: "emp",
                table: "Image",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Image",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Image",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "History_Stop",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "History_Stop",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsComputable",
                schema: "emp",
                table: "History_Stop",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "History_Stop",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "HistoryStopDays",
                schema: "emp",
                table: "History_Stop",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "History_Stop",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                schema: "emp",
                table: "Group_Punishment_Encourage_File",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "MimeType",
                schema: "emp",
                table: "Group_Punishment_Encourage_File",
                type: "nvarchar(max)",
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Group_Punishment_Encourage_File",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Group_Punishment_Encourage_File",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Group_Punishment_Encourage_File",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Extension",
                schema: "emp",
                table: "Group_Punishment_Encourage_File",
                type: "nvarchar(max)",
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Group_Punishment_Encourage_File",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                schema: "emp",
                table: "Group_Punishment_Encourage",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedUser",
                schema: "emp",
                table: "Group_Punishment_Encourage",
                type: "nvarchar(max)",
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Group_Punishment_Encourage",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Group_Punishment_Encourage",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Group_Punishment_Encourage",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EmPloyeeCount",
                schema: "emp",
                table: "Group_Punishment_Encourage",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Group_Punishment_Encourage",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 4096,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Group_Punishment_Encourage",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MissionSubject",
                schema: "emp",
                table: "Foreign_Travel",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MissionCost",
                schema: "emp",
                table: "Foreign_Travel",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LetterNumber",
                schema: "emp",
                table: "Foreign_Travel",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Foreign_Travel",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Foreign_Travel",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Foreign_Travel",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descriptions",
                schema: "emp",
                table: "Foreign_Travel",
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
                table: "Foreign_Travel",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CountryNames",
                schema: "emp",
                table: "Foreign_Travel",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CountryList",
                schema: "emp",
                table: "Foreign_Travel",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CountryCount",
                schema: "emp",
                table: "Foreign_Travel",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ArchiveId",
                schema: "emp",
                table: "Foreign_Travel",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OtherLanguageName",
                schema: "emp",
                table: "Foreign_Language",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Foreign_Language",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Languagescore",
                schema: "emp",
                table: "Foreign_Language",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Foreign_Language",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Foreign_Language",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Foreign_Language",
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
                table: "Foreign_Language",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Acceptable",
                schema: "emp",
                table: "Foreign_Language",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                schema: "emp",
                table: "File",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "MimeType",
                schema: "emp",
                table: "File",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "File",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "File",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "File",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "File",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "UsedinOrder",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NationalNo",
                schema: "emp",
                table: "Family",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "MaintenanceCost",
                schema: "emp",
                table: "Family",
                type: "real",
                nullable: true,
                defaultValueSql: "((0))",
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                schema: "emp",
                table: "Family",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Family",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsWelfareServices",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsVerify",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPremierStudent",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsLast",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsImperfective",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsHekmat",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDependent",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCoveredInsurance",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsCashBenefits",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsuranceNumber",
                schema: "emp",
                table: "Family",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdentityNo",
                schema: "emp",
                table: "Family",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Family",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "HasCertification",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                schema: "emp",
                table: "Family",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FatherName",
                schema: "emp",
                table: "Family",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "EffectivePercent",
                schema: "emp",
                table: "Family",
                type: "decimal(18,2)",
                nullable: true,
                defaultValueSql: "((0))",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DisabilityPercent",
                schema: "emp",
                table: "Family",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Family",
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
                table: "Family",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountNumber",
                schema: "emp",
                table: "Family",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Experience",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsInternal",
                schema: "emp",
                table: "Experience",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Experience",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAcceptable",
                schema: "emp",
                table: "Experience",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Experience",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Duration",
                schema: "emp",
                table: "Experience",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(6)",
                oldMaxLength: 6,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Experience",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyTitle",
                schema: "emp",
                table: "Experience",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AcceptablePercent",
                schema: "emp",
                table: "Experience",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "YearCoefficent",
                schema: "emp",
                table: "Evaluation_Result",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                schema: "emp",
                table: "Evaluation_Result",
                type: "int",
                nullable: false,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Evaluation_Result",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Evaluation_Result",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Evaluation_Result",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "EvaluationCoefficent",
                schema: "emp",
                table: "Evaluation_Result",
                type: "tinyint",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Evaluation_Result",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Average",
                schema: "emp",
                table: "Evaluation_Result",
                type: "decimal(18,2)",
                nullable: true,
                defaultValueSql: "((0))",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                schema: "emp",
                table: "EmployeeRefreshToken",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "RevokedByIp",
                schema: "emp",
                table: "EmployeeRefreshToken",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(45)",
                oldMaxLength: 45,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RevocationReason",
                schema: "emp",
                table: "EmployeeRefreshToken",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReplacedByToken",
                schema: "emp",
                table: "EmployeeRefreshToken",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByIp",
                schema: "emp",
                table: "EmployeeRefreshToken",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(45)",
                oldMaxLength: 45,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Purpose",
                schema: "emp",
                table: "EmployeeOtp",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32);

            migrationBuilder.AlterColumn<bool>(
                name: "IsUsed",
                schema: "emp",
                table: "EmployeeOtp",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByIp",
                schema: "emp",
                table: "EmployeeOtp",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(45)",
                oldMaxLength: 45,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CodeHash",
                schema: "emp",
                table: "EmployeeOtp",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Employee_Software",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Employee_Software",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Employee_Software",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Employee_Software",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                schema: "emp",
                table: "Employee_Request_Detail",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Employee_Request_Detail",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Employee_Request_Detail",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Employee_Request_Detail",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Employee_Request_Detail",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Employee_Request_Detail",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                schema: "emp",
                table: "Employee_Request",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Employee_Request",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Employee_Request",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Employee_Request",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Employee_Request",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Employee_Request",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Employee_Login_History",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsSuccess",
                schema: "emp",
                table: "Employee_Login_History",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Employee_Login_History",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Employee_Login_History",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Employee_Login_History",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OtherFileGroupName",
                schema: "emp",
                table: "Employee_File",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "emp",
                table: "Employee_File",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Employee_File",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsImage",
                schema: "emp",
                table: "Employee_File",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Employee_File",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Employee_File",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Employee_File",
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
                table: "Employee_File",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ThesisTitle",
                schema: "emp",
                table: "Education",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "SetByEmployee",
                schema: "emp",
                table: "Education",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OtherUniversityName",
                schema: "emp",
                table: "Education",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LicenceNumber",
                schema: "emp",
                table: "Education",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Education",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsUsedInOrder",
                schema: "emp",
                table: "Education",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsInDutyTime",
                schema: "emp",
                table: "Education",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Education",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDefaultEducation",
                schema: "emp",
                table: "Education",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsBoursie",
                schema: "emp",
                table: "Education",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Education",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EducationAverage",
                schema: "emp",
                table: "Education",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descriptions",
                schema: "emp",
                table: "Education",
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
                table: "Education",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PreviousDerivingNumber",
                schema: "emp",
                table: "Driving_License",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LicenseSerialNumber",
                schema: "emp",
                table: "Driving_License",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Licencedescription",
                schema: "emp",
                table: "Driving_License",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Driving_License",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Driving_License",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Driving_License",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Driving_License",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Disability",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsLast",
                schema: "emp",
                table: "Disability",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Disability",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Disability",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "HasCertification",
                schema: "emp",
                table: "Disability",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DisabilityPercent",
                schema: "emp",
                table: "Disability",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Disability",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Course",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Course",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Course",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Course",
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
                table: "Course",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CoursepPlace",
                schema: "emp",
                table: "Course",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CourseTime",
                schema: "emp",
                table: "Course",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CourseSession",
                schema: "emp",
                table: "Course",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CourseSerial",
                schema: "emp",
                table: "Course",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(16)",
                oldMaxLength: 16,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CourseMark",
                schema: "emp",
                table: "Course",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Zipcode",
                schema: "emp",
                table: "Contact_Info",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                schema: "emp",
                table: "Contact_Info",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MobileNo",
                schema: "emp",
                table: "Contact_Info",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Mail",
                schema: "emp",
                table: "Contact_Info",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Contact_Info",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsVerify",
                schema: "emp",
                table: "Contact_Info",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLast",
                schema: "emp",
                table: "Contact_Info",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Contact_Info",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Contact_Info",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Fax",
                schema: "emp",
                table: "Contact_Info",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmergencyPhone",
                schema: "emp",
                table: "Contact_Info",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Contact_Info",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "emp",
                table: "Contact_Info",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Competency",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Competency",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Competency",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Competency",
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
                table: "Competency",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Acceptable",
                schema: "emp",
                table: "Competency",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                schema: "emp",
                table: "Coefficient",
                type: "decimal(18,2)",
                nullable: true,
                defaultValueSql: "((0))",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Coefficient",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Coefficient",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Coefficient",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Coefficient",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Character",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Character",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Character",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Character",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                schema: "emp",
                table: "Captivity",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrackingCode",
                schema: "emp",
                table: "Captivity",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "SacrificePercent",
                schema: "emp",
                table: "Captivity",
                type: "float",
                nullable: true,
                defaultValueSql: "((0))",
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Month",
                schema: "emp",
                table: "Captivity",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LetterNumber",
                schema: "emp",
                table: "Captivity",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Captivity",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Captivity",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsContinues",
                schema: "emp",
                table: "Captivity",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "emp",
                table: "Captivity",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Captivity",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Captivity",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Day",
                schema: "emp",
                table: "Captivity",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Captivity",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                schema: "emp",
                table: "Basij_Grade",
                type: "int",
                nullable: false,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Basij_Grade",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Basij_Grade",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Basij_Grade",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Grade",
                schema: "emp",
                table: "Basij_Grade",
                type: "int",
                nullable: false,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Basij_Grade",
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
                table: "Basij_Grade",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "YearCoefficient",
                schema: "emp",
                table: "Basij",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                schema: "emp",
                table: "Basij",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrackingCode",
                schema: "emp",
                table: "Basij",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LetterNumber",
                schema: "emp",
                table: "Basij",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Basij",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsPercent",
                schema: "emp",
                table: "Basij",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Basij",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsContinues",
                schema: "emp",
                table: "Basij",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsComputeableInHistory",
                schema: "emp",
                table: "Basij",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "emp",
                table: "Basij",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Basij",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DurationYear",
                schema: "emp",
                table: "Basij",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DurationMonth",
                schema: "emp",
                table: "Basij",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DurationDay",
                schema: "emp",
                table: "Basij",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descriptions",
                schema: "emp",
                table: "Basij",
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
                table: "Basij",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ToPrice",
                schema: "emp",
                table: "Bank_Account",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                schema: "emp",
                table: "Bank_Account",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "ShabaNumber",
                schema: "emp",
                table: "Bank_Account",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                schema: "emp",
                table: "Bank_Account",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OldId",
                schema: "emp",
                table: "Bank_Account",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Bank_Account",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Bank_Account",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Bank_Account",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FromPrice",
                schema: "emp",
                table: "Bank_Account",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Bank_Account",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Bank_Account",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CardNumber",
                schema: "emp",
                table: "Bank_Account",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BonCardNumber",
                schema: "emp",
                table: "Bank_Account",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BankBranchId",
                schema: "emp",
                table: "Bank_Account",
                type: "int",
                nullable: true,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountNumber",
                schema: "emp",
                table: "Bank_Account",
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
                table: "Attendance",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Attendance",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "InOutType",
                schema: "emp",
                table: "Attendance",
                type: "int",
                nullable: false,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "InOutCard",
                schema: "emp",
                table: "Attendance",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Attendance",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceName",
                schema: "emp",
                table: "Attendance",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Attendance",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Weight",
                schema: "emp",
                table: "Appearance",
                type: "int",
                nullable: false,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "SpecificSymptoms",
                schema: "emp",
                table: "Appearance",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Appearance",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Appearance",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Appearance",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Height",
                schema: "emp",
                table: "Appearance",
                type: "int",
                nullable: false,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FootSize",
                schema: "emp",
                table: "Appearance",
                type: "int",
                nullable: false,
                defaultValueSql: "(0)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Appearance",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "SecondApprove",
                schema: "emp",
                table: "Absence_Record",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Absence_Record",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Absence_Record",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Absence_Record",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "FirstApprove",
                schema: "emp",
                table: "Absence_Record",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Absence_Record",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Ability",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Ability",
                type: "bit",
                nullable: false,
                defaultValueSql: "(CONVERT([bit],(0)))",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Ability",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Ability",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                defaultValueSql: "(N'')",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "title",
                schema: "emp",
                table: "Work",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "YearMult",
                schema: "emp",
                table: "Work",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "WorkPlaceDesc",
                schema: "emp",
                table: "Work",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "RetiredMult",
                schema: "emp",
                table: "Work",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "LetterNumber",
                schema: "emp",
                table: "Work",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastTitle",
                schema: "emp",
                table: "Work",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Work",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Work",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsComputeable",
                schema: "emp",
                table: "Work",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<byte>(
                name: "InsHsyYear",
                schema: "emp",
                table: "Work",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<byte>(
                name: "InsHsyMonth",
                schema: "emp",
                table: "Work",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<byte>(
                name: "InsHsyDay",
                schema: "emp",
                table: "Work",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Work",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "ExperienceMult",
                schema: "emp",
                table: "Work",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Work",
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
                table: "Work",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<byte>(
                name: "AcptInsHsyYear",
                schema: "emp",
                table: "Work",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<byte>(
                name: "AcptInsHsyMonth",
                schema: "emp",
                table: "Work",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<byte>(
                name: "AcptInsHsyDay",
                schema: "emp",
                table: "Work",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "TrackingCode",
                schema: "emp",
                table: "War",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<double>(
                name: "PercentAnnualIncrease",
                schema: "emp",
                table: "War",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true,
                oldDefaultValueSql: "((0))");

            migrationBuilder.AlterColumn<string>(
                name: "LetterNumber",
                schema: "emp",
                table: "War",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "War",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "JebheOperations",
                schema: "emp",
                table: "War",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "War",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsContinues",
                schema: "emp",
                table: "War",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "emp",
                table: "War",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "War",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "DurationYear",
                schema: "emp",
                table: "War",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<int>(
                name: "DurationMonth",
                schema: "emp",
                table: "War",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<int>(
                name: "DurationDay",
                schema: "emp",
                table: "War",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "Descriptions",
                schema: "emp",
                table: "War",
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
                table: "War",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "AcceptableDurationForTaxExemption",
                schema: "emp",
                table: "War",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "Value",
                schema: "emp",
                table: "Temp_Punishment_Encourage",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<int>(
                name: "UnitValue",
                schema: "emp",
                table: "Temp_Punishment_Encourage",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "NationalNo",
                schema: "emp",
                table: "Temp_Punishment_Encourage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Temp_Punishment_Encourage",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Temp_Punishment_Encourage",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Temp_Punishment_Encourage",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Temp_Punishment_Encourage",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "Value",
                schema: "emp",
                table: "Punishment_Encourage",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<int>(
                name: "UnitValue",
                schema: "emp",
                table: "Punishment_Encourage",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Punishment_Encourage",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsGroup",
                schema: "emp",
                table: "Punishment_Encourage",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Punishment_Encourage",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Punishment_Encourage",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Punishment_Encourage",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 4096,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Punishment_Encourage",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "TrackingCode",
                schema: "emp",
                table: "Other_Veteran",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "SacrificePercent",
                schema: "emp",
                table: "Other_Veteran",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "LetterNumber",
                schema: "emp",
                table: "Other_Veteran",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Other_Veteran",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLast",
                schema: "emp",
                table: "Other_Veteran",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Other_Veteran",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsComputeable",
                schema: "emp",
                table: "Other_Veteran",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "emp",
                table: "Other_Veteran",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Other_Veteran",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "DurationYear",
                schema: "emp",
                table: "Other_Veteran",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<int>(
                name: "DurationMonth",
                schema: "emp",
                table: "Other_Veteran",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<int>(
                name: "DurationDay",
                schema: "emp",
                table: "Other_Veteran",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "Descriptions",
                schema: "emp",
                table: "Other_Veteran",
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
                table: "Other_Veteran",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "NameOfPeriod",
                schema: "emp",
                table: "Military_Service",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "MilitaryMinDuration",
                schema: "emp",
                table: "Military_Service",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "MilitaryFullDuration",
                schema: "emp",
                table: "Military_Service",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "MilitaryDuration",
                schema: "emp",
                table: "Military_Service",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "MilitariSerialNo",
                schema: "emp",
                table: "Military_Service",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Military_Service",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLast",
                schema: "emp",
                table: "Military_Service",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Military_Service",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsContinue",
                schema: "emp",
                table: "Military_Service",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsComputable",
                schema: "emp",
                table: "Military_Service",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Military_Service",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Descriptions",
                schema: "emp",
                table: "Military_Service",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Military_Service",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "ConfirmedLetterNo",
                schema: "emp",
                table: "Military_Service",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "TrackingCode",
                schema: "emp",
                table: "Isar",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LetterNumber",
                schema: "emp",
                table: "Isar",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Isar",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<float>(
                name: "Isarpercent",
                schema: "emp",
                table: "Isar",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true,
                oldDefaultValueSql: "((0))");

            migrationBuilder.AlterColumn<string>(
                name: "IsarInjuerdOrgan",
                schema: "emp",
                table: "Isar",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "IsarInability",
                schema: "emp",
                table: "Isar",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "IsarDurationYear",
                schema: "emp",
                table: "Isar",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<int>(
                name: "IsarDurationMonth",
                schema: "emp",
                table: "Isar",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<int>(
                name: "IsarDurationDay",
                schema: "emp",
                table: "Isar",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Isar",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsContinues",
                schema: "emp",
                table: "Isar",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "emp",
                table: "Isar",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Isar",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Isar",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Isar",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                schema: "emp",
                table: "Insurance_Detail",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<int>(
                name: "Month",
                schema: "emp",
                table: "Insurance_Detail",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Insurance_Detail",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsOptionalInsurnce",
                schema: "emp",
                table: "Insurance_Detail",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsFullInsurnce",
                schema: "emp",
                table: "Insurance_Detail",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Insurance_Detail",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsComputable",
                schema: "emp",
                table: "Insurance_Detail",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Insurance_Detail",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Desc",
                schema: "emp",
                table: "Insurance_Detail",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Insurance_Detail",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "AccDay",
                schema: "emp",
                table: "Insurance_Detail",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Insurance",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLast",
                schema: "emp",
                table: "Insurance",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Insurance",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "InsuranceNumber",
                schema: "emp",
                table: "Insurance",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Insurance",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "HasSupplementaryInsurance",
                schema: "emp",
                table: "Insurance",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Insurance",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "AccDay",
                schema: "emp",
                table: "Insurance",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Image",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Image",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDefault",
                schema: "emp",
                table: "Image",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Image",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Image",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "History_Stop",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "History_Stop",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsComputable",
                schema: "emp",
                table: "History_Stop",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "History_Stop",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "HistoryStopDays",
                schema: "emp",
                table: "History_Stop",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "History_Stop",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                schema: "emp",
                table: "Group_Punishment_Encourage_File",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "MimeType",
                schema: "emp",
                table: "Group_Punishment_Encourage_File",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Group_Punishment_Encourage_File",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Group_Punishment_Encourage_File",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Group_Punishment_Encourage_File",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Extension",
                schema: "emp",
                table: "Group_Punishment_Encourage_File",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Group_Punishment_Encourage_File",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                schema: "emp",
                table: "Group_Punishment_Encourage",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedUser",
                schema: "emp",
                table: "Group_Punishment_Encourage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Group_Punishment_Encourage",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Group_Punishment_Encourage",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Group_Punishment_Encourage",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "EmPloyeeCount",
                schema: "emp",
                table: "Group_Punishment_Encourage",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Group_Punishment_Encourage",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 4096,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Group_Punishment_Encourage",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "MissionSubject",
                schema: "emp",
                table: "Foreign_Travel",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "MissionCost",
                schema: "emp",
                table: "Foreign_Travel",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "LetterNumber",
                schema: "emp",
                table: "Foreign_Travel",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Foreign_Travel",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Foreign_Travel",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Foreign_Travel",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Descriptions",
                schema: "emp",
                table: "Foreign_Travel",
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
                table: "Foreign_Travel",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CountryNames",
                schema: "emp",
                table: "Foreign_Travel",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CountryList",
                schema: "emp",
                table: "Foreign_Travel",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "CountryCount",
                schema: "emp",
                table: "Foreign_Travel",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<int>(
                name: "ArchiveId",
                schema: "emp",
                table: "Foreign_Travel",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "OtherLanguageName",
                schema: "emp",
                table: "Foreign_Language",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Foreign_Language",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Languagescore",
                schema: "emp",
                table: "Foreign_Language",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Foreign_Language",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Foreign_Language",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Foreign_Language",
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
                table: "Foreign_Language",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "Acceptable",
                schema: "emp",
                table: "Foreign_Language",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                schema: "emp",
                table: "File",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "MimeType",
                schema: "emp",
                table: "File",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "File",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "File",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "File",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "File",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "UsedinOrder",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "NationalNo",
                schema: "emp",
                table: "Family",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<float>(
                name: "MaintenanceCost",
                schema: "emp",
                table: "Family",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true,
                oldDefaultValueSql: "((0))");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                schema: "emp",
                table: "Family",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Family",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsWelfareServices",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsVerify",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPremierStudent",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLast",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsImperfective",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsHekmat",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDependent",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCoveredInsurance",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCashBenefits",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "InsuranceNumber",
                schema: "emp",
                table: "Family",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityNo",
                schema: "emp",
                table: "Family",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Family",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "HasCertification",
                schema: "emp",
                table: "Family",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                schema: "emp",
                table: "Family",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "FatherName",
                schema: "emp",
                table: "Family",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<decimal>(
                name: "EffectivePercent",
                schema: "emp",
                table: "Family",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true,
                oldDefaultValueSql: "((0))");

            migrationBuilder.AlterColumn<int>(
                name: "DisabilityPercent",
                schema: "emp",
                table: "Family",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Family",
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
                table: "Family",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "AccountNumber",
                schema: "emp",
                table: "Family",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Experience",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsInternal",
                schema: "emp",
                table: "Experience",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Experience",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAcceptable",
                schema: "emp",
                table: "Experience",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Experience",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Duration",
                schema: "emp",
                table: "Experience",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(6)",
                oldMaxLength: 6,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Experience",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyTitle",
                schema: "emp",
                table: "Experience",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "AcceptablePercent",
                schema: "emp",
                table: "Experience",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<int>(
                name: "YearCoefficent",
                schema: "emp",
                table: "Evaluation_Result",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                schema: "emp",
                table: "Evaluation_Result",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Evaluation_Result",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Evaluation_Result",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Evaluation_Result",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<byte>(
                name: "EvaluationCoefficent",
                schema: "emp",
                table: "Evaluation_Result",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Evaluation_Result",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<decimal>(
                name: "Average",
                schema: "emp",
                table: "Evaluation_Result",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true,
                oldDefaultValueSql: "((0))");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                schema: "emp",
                table: "EmployeeRefreshToken",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "RevokedByIp",
                schema: "emp",
                table: "EmployeeRefreshToken",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(45)",
                oldMaxLength: 45,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "RevocationReason",
                schema: "emp",
                table: "EmployeeRefreshToken",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "ReplacedByToken",
                schema: "emp",
                table: "EmployeeRefreshToken",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByIp",
                schema: "emp",
                table: "EmployeeRefreshToken",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(45)",
                oldMaxLength: 45,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Purpose",
                schema: "emp",
                table: "EmployeeOtp",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsUsed",
                schema: "emp",
                table: "EmployeeOtp",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByIp",
                schema: "emp",
                table: "EmployeeOtp",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(45)",
                oldMaxLength: 45,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CodeHash",
                schema: "emp",
                table: "EmployeeOtp",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Employee_Software",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Employee_Software",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Employee_Software",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Employee_Software",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                schema: "emp",
                table: "Employee_Request_Detail",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Employee_Request_Detail",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Employee_Request_Detail",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Employee_Request_Detail",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Employee_Request_Detail",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Employee_Request_Detail",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                schema: "emp",
                table: "Employee_Request",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Employee_Request",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Employee_Request",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Employee_Request",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Employee_Request",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Employee_Request",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Employee_Login_History",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSuccess",
                schema: "emp",
                table: "Employee_Login_History",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Employee_Login_History",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Employee_Login_History",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Employee_Login_History",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "OtherFileGroupName",
                schema: "emp",
                table: "Employee_File",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "emp",
                table: "Employee_File",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Employee_File",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsImage",
                schema: "emp",
                table: "Employee_File",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Employee_File",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Employee_File",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Employee_File",
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
                table: "Employee_File",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "ThesisTitle",
                schema: "emp",
                table: "Education",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "SetByEmployee",
                schema: "emp",
                table: "Education",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "OtherUniversityName",
                schema: "emp",
                table: "Education",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LicenceNumber",
                schema: "emp",
                table: "Education",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Education",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsUsedInOrder",
                schema: "emp",
                table: "Education",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsInDutyTime",
                schema: "emp",
                table: "Education",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Education",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDefaultEducation",
                schema: "emp",
                table: "Education",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsBoursie",
                schema: "emp",
                table: "Education",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Education",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "EducationAverage",
                schema: "emp",
                table: "Education",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Descriptions",
                schema: "emp",
                table: "Education",
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
                table: "Education",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "PreviousDerivingNumber",
                schema: "emp",
                table: "Driving_License",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LicenseSerialNumber",
                schema: "emp",
                table: "Driving_License",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Licencedescription",
                schema: "emp",
                table: "Driving_License",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Driving_License",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Driving_License",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Driving_License",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Driving_License",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Disability",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLast",
                schema: "emp",
                table: "Disability",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Disability",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Disability",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "HasCertification",
                schema: "emp",
                table: "Disability",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<int>(
                name: "DisabilityPercent",
                schema: "emp",
                table: "Disability",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Disability",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Course",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Course",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Course",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Course",
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
                table: "Course",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CoursepPlace",
                schema: "emp",
                table: "Course",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "CourseTime",
                schema: "emp",
                table: "Course",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<int>(
                name: "CourseSession",
                schema: "emp",
                table: "Course",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "CourseSerial",
                schema: "emp",
                table: "Course",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(16)",
                oldMaxLength: 16,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CourseMark",
                schema: "emp",
                table: "Course",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Zipcode",
                schema: "emp",
                table: "Contact_Info",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                schema: "emp",
                table: "Contact_Info",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "MobileNo",
                schema: "emp",
                table: "Contact_Info",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Mail",
                schema: "emp",
                table: "Contact_Info",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Contact_Info",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsVerify",
                schema: "emp",
                table: "Contact_Info",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLast",
                schema: "emp",
                table: "Contact_Info",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Contact_Info",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Contact_Info",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Fax",
                schema: "emp",
                table: "Contact_Info",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "EmergencyPhone",
                schema: "emp",
                table: "Contact_Info",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Contact_Info",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "emp",
                table: "Contact_Info",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Competency",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Competency",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Competency",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Competency",
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
                table: "Competency",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "Acceptable",
                schema: "emp",
                table: "Competency",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                schema: "emp",
                table: "Coefficient",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true,
                oldDefaultValueSql: "((0))");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Coefficient",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Coefficient",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Coefficient",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Coefficient",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Character",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Character",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Character",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Character",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                schema: "emp",
                table: "Captivity",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "TrackingCode",
                schema: "emp",
                table: "Captivity",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<double>(
                name: "SacrificePercent",
                schema: "emp",
                table: "Captivity",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true,
                oldDefaultValueSql: "((0))");

            migrationBuilder.AlterColumn<int>(
                name: "Month",
                schema: "emp",
                table: "Captivity",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "LetterNumber",
                schema: "emp",
                table: "Captivity",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Captivity",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Captivity",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsContinues",
                schema: "emp",
                table: "Captivity",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "emp",
                table: "Captivity",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Captivity",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Captivity",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "Day",
                schema: "emp",
                table: "Captivity",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Captivity",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                schema: "emp",
                table: "Basij_Grade",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Basij_Grade",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Basij_Grade",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Basij_Grade",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "Grade",
                schema: "emp",
                table: "Basij_Grade",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Basij_Grade",
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
                table: "Basij_Grade",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "YearCoefficient",
                schema: "emp",
                table: "Basij",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                schema: "emp",
                table: "Basij",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "TrackingCode",
                schema: "emp",
                table: "Basij",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LetterNumber",
                schema: "emp",
                table: "Basij",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Basij",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPercent",
                schema: "emp",
                table: "Basij",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Basij",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsContinues",
                schema: "emp",
                table: "Basij",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsComputeableInHistory",
                schema: "emp",
                table: "Basij",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "emp",
                table: "Basij",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Basij",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "DurationYear",
                schema: "emp",
                table: "Basij",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<int>(
                name: "DurationMonth",
                schema: "emp",
                table: "Basij",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<int>(
                name: "DurationDay",
                schema: "emp",
                table: "Basij",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "Descriptions",
                schema: "emp",
                table: "Basij",
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
                table: "Basij",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "ToPrice",
                schema: "emp",
                table: "Bank_Account",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                schema: "emp",
                table: "Bank_Account",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "ShabaNumber",
                schema: "emp",
                table: "Bank_Account",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                schema: "emp",
                table: "Bank_Account",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<int>(
                name: "OldId",
                schema: "emp",
                table: "Bank_Account",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Bank_Account",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Bank_Account",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Bank_Account",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "FromPrice",
                schema: "emp",
                table: "Bank_Account",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "emp",
                table: "Bank_Account",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Bank_Account",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CardNumber",
                schema: "emp",
                table: "Bank_Account",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "BonCardNumber",
                schema: "emp",
                table: "Bank_Account",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "BankBranchId",
                schema: "emp",
                table: "Bank_Account",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "AccountNumber",
                schema: "emp",
                table: "Bank_Account",
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
                table: "Attendance",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Attendance",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<int>(
                name: "InOutType",
                schema: "emp",
                table: "Attendance",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "InOutCard",
                schema: "emp",
                table: "Attendance",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Attendance",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceName",
                schema: "emp",
                table: "Attendance",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Attendance",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AddColumn<string>(
                name: "title",
                schema: "emp",
                table: "Attendance",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Weight",
                schema: "emp",
                table: "Appearance",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "SpecificSymptoms",
                schema: "emp",
                table: "Appearance",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Appearance",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Appearance",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Appearance",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<int>(
                name: "Height",
                schema: "emp",
                table: "Appearance",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<int>(
                name: "FootSize",
                schema: "emp",
                table: "Appearance",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "(0)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Appearance",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "SecondApprove",
                schema: "emp",
                table: "Absence_Record",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Absence_Record",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Absence_Record",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Absence_Record",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "FirstApprove",
                schema: "emp",
                table: "Absence_Record",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Absence_Record",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AddColumn<string>(
                name: "title",
                schema: "emp",
                table: "Absence_Record",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "emp",
                table: "Ability",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "emp",
                table: "Ability",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "emp",
                table: "Ability",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "emp",
                table: "Ability",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldDefaultValueSql: "(N'')");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Report.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Microssssrvivesds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
     

            migrationBuilder.CreateTable(
                name: "Field_DataType",
                schema: "rpt",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FriendlyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Field_DataType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reportable_Entity",
                schema: "rpt",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicalName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FriendlyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Schema = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reportable_Entity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Field_Operator",
                schema: "rpt",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FieldDataTypeId = table.Column<int>(type: "int", nullable: false),
                    Operator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FriendlyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FieldDataTypeId1 = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Field_Operator", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Field_Operator_Field_DataType_FieldDataTypeId1",
                        column: x => x.FieldDataTypeId1,
                        principalSchema: "rpt",
                        principalTable: "Field_DataType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reportable_Field",
                schema: "rpt",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportableEntityId = table.Column<long>(type: "bigint", nullable: false),
                    TechnicalName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FriendlyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FieldDataTypeId = table.Column<int>(type: "int", nullable: false),
                    NavigationPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFilterable = table.Column<bool>(type: "bit", nullable: false),
                    IsSelectable = table.Column<bool>(type: "bit", nullable: false),
                    IsSortable = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FieldDataTypeId1 = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reportable_Field", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reportable_Field_Field_DataType_FieldDataTypeId1",
                        column: x => x.FieldDataTypeId1,
                        principalSchema: "rpt",
                        principalTable: "Field_DataType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reportable_Field_Reportable_Entity_ReportableEntityId",
                        column: x => x.ReportableEntityId,
                        principalSchema: "rpt",
                        principalTable: "Reportable_Entity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Field_Operator_FieldDataTypeId1",
                schema: "rpt",
                table: "Field_Operator",
                column: "FieldDataTypeId1");

            migrationBuilder.CreateIndex(
                name: "IX_Reportable_Field_FieldDataTypeId1",
                schema: "rpt",
                table: "Reportable_Field",
                column: "FieldDataTypeId1");

            migrationBuilder.CreateIndex(
                name: "IX_Reportable_Field_ReportableEntityId",
                schema: "rpt",
                table: "Reportable_Field",
                column: "ReportableEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Field_Operator",
                schema: "rpt");

            migrationBuilder.DropTable(
                name: "Reportable_Field",
                schema: "rpt");

            migrationBuilder.DropTable(
                name: "Field_DataType",
                schema: "rpt");

            migrationBuilder.DropTable(
                name: "Reportable_Entity",
                schema: "rpt");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Chart_OrgTypeId",
                schema: "Org",
                table: "Organisation_Chart",
                column: "OrgTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organisation_Chart_Base_Table_Value_OrgTypeId",
                schema: "Org",
                table: "Organisation_Chart",
                column: "OrgTypeId",
                principalSchema: "bas",
                principalTable: "Base_Table_Value",
                principalColumn: "Id");
        }
    }
}

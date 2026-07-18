using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.BaseInfo.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExcelDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Import_Profile",
                schema: "bas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TargetEntityName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    TargetSchema = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ModuleKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    HandlerType = table.Column<int>(type: "int", nullable: false),
                    CustomHandlerKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    RequiresEmployeeLookup = table.Column<bool>(type: "bit", nullable: false),
                    AllowedExtensions = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    MaxRowCount = table.Column<int>(type: "int", nullable: false),
                    HasHeaderRow = table.Column<bool>(type: "bit", nullable: false),
                    PermissionKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Import_Profile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Import_Batch",
                schema: "bas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportProfileId = table.Column<long>(type: "bigint", nullable: false),
                    FileId = table.Column<long>(type: "bigint", nullable: false),
                    ContextJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalRowsRead = table.Column<int>(type: "int", nullable: false),
                    ValidCount = table.Column<int>(type: "int", nullable: false),
                    WarningCount = table.Column<int>(type: "int", nullable: false),
                    ErrorCount = table.Column<int>(type: "int", nullable: false),
                    InsertedCount = table.Column<int>(type: "int", nullable: false),
                    FailedRowsJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UploaderUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UploaderDisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Import_Batch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Import_Batch_Import_Profile_ImportProfileId",
                        column: x => x.ImportProfileId,
                        principalSchema: "bas",
                        principalTable: "Import_Profile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Import_Profile_Field",
                schema: "bas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportProfileId = table.Column<long>(type: "bigint", nullable: false),
                    ExcelColumnOrder = table.Column<int>(type: "int", nullable: false),
                    ExcelColumnLetter = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    ExcelColumnHeader = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TargetPropertyName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsUniqueKey = table.Column<bool>(type: "bit", nullable: false),
                    FkLookupType = table.Column<int>(type: "int", nullable: false),
                    FkReferenceEntity = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    FkReferenceField = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Import_Profile_Field", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Import_Profile_Field_Import_Profile_ImportProfileId",
                        column: x => x.ImportProfileId,
                        principalSchema: "bas",
                        principalTable: "Import_Profile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Import_Temp_Row",
                schema: "bas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportBatchId = table.Column<long>(type: "bigint", nullable: false),
                    RowNumber = table.Column<int>(type: "int", nullable: false),
                    RawDataJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ParsedDataJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ResolvedDataJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ValidationStatus = table.Column<int>(type: "int", nullable: false),
                    ValidationMessagesJson = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MainRecordId = table.Column<long>(type: "bigint", nullable: true),
                    FinalizedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Import_Temp_Row", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Import_Temp_Row_Import_Batch_ImportBatchId",
                        column: x => x.ImportBatchId,
                        principalSchema: "bas",
                        principalTable: "Import_Batch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Import_Batch_ImportProfileId",
                schema: "bas",
                table: "Import_Batch",
                column: "ImportProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Import_Profile_Field_ImportProfileId",
                schema: "bas",
                table: "Import_Profile_Field",
                column: "ImportProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Import_Temp_Row_ImportBatchId",
                schema: "bas",
                table: "Import_Temp_Row",
                column: "ImportBatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Import_Profile_Field",
                schema: "bas");

            migrationBuilder.DropTable(
                name: "Import_Temp_Row",
                schema: "bas");

            migrationBuilder.DropTable(
                name: "Import_Batch",
                schema: "bas");

            migrationBuilder.DropTable(
                name: "Import_Profile",
                schema: "bas");
        }
    }
}

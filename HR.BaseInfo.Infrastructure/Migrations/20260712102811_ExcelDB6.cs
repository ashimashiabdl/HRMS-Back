using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.BaseInfo.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExcelDB6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Some databases never had these FKs/indexes (e.g. scaffolded without constraints).
            migrationBuilder.Sql("""
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Education_Field_Education_Group_EducationGroupId' AND parent_object_id = OBJECT_ID(N'bas.Education_Field'))
                    ALTER TABLE [bas].[Education_Field] DROP CONSTRAINT [FK_Education_Field_Education_Group_EducationGroupId];

                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Education_Orientation_Education_Field_EducationFieldId' AND parent_object_id = OBJECT_ID(N'bas.Education_Orientation'))
                    ALTER TABLE [bas].[Education_Orientation] DROP CONSTRAINT [FK_Education_Orientation_Education_Field_EducationFieldId];

                IF OBJECT_ID(N'bas.Education_Grade_Field', N'U') IS NOT NULL
                    DROP TABLE [bas].[Education_Grade_Field];

                IF OBJECT_ID(N'bas.EducationGrade_Orientation', N'U') IS NOT NULL
                    DROP TABLE [bas].[EducationGrade_Orientation];

                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Education_Orientation_EducationFieldId_title' AND object_id = OBJECT_ID(N'bas.Education_Orientation'))
                    DROP INDEX [IX_Education_Orientation_EducationFieldId_title] ON [bas].[Education_Orientation];

                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Education_Field_EducationGroupId_title' AND object_id = OBJECT_ID(N'bas.Education_Field'))
                    DROP INDEX [IX_Education_Field_EducationGroupId_title] ON [bas].[Education_Field];

                IF COL_LENGTH(N'bas.Education_Orientation', N'EducationFieldId') IS NOT NULL
                    ALTER TABLE [bas].[Education_Orientation] DROP COLUMN [EducationFieldId];

                IF COL_LENGTH(N'bas.Education_Field', N'EducationGroupId') IS NOT NULL
                    ALTER TABLE [bas].[Education_Field] DROP COLUMN [EducationGroupId];
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EducationFieldId",
                schema: "bas",
                table: "Education_Orientation",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "EducationGroupId",
                schema: "bas",
                table: "Education_Field",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Education_Grade_Field",
                schema: "bas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EducationFieldId = table.Column<long>(type: "bigint", nullable: false),
                    EducationGradeId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    OldID = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Education_Grade_Field", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Education_Grade_Field_Education_Field_EducationFieldId",
                        column: x => x.EducationFieldId,
                        principalSchema: "bas",
                        principalTable: "Education_Field",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Education_Grade_Field_Education_Grade_EducationGradeId",
                        column: x => x.EducationGradeId,
                        principalSchema: "bas",
                        principalTable: "Education_Grade",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EducationGrade_Orientation",
                schema: "bas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EducationGradeId = table.Column<long>(type: "bigint", nullable: false),
                    EducationOrientationId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    OldID = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationGrade_Orientation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationGrade_Orientation_Education_Grade_EducationGradeId",
                        column: x => x.EducationGradeId,
                        principalSchema: "bas",
                        principalTable: "Education_Grade",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EducationGrade_Orientation_Education_Orientation_EducationOrientationId",
                        column: x => x.EducationOrientationId,
                        principalSchema: "bas",
                        principalTable: "Education_Orientation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Education_Orientation_EducationFieldId_title",
                schema: "bas",
                table: "Education_Orientation",
                columns: new[] { "EducationFieldId", "title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Education_Field_EducationGroupId_title",
                schema: "bas",
                table: "Education_Field",
                columns: new[] { "EducationGroupId", "title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Education_Grade_Field_EducationFieldId",
                schema: "bas",
                table: "Education_Grade_Field",
                column: "EducationFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_Education_Grade_Field_EducationGradeId",
                schema: "bas",
                table: "Education_Grade_Field",
                column: "EducationGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationGrade_Orientation_EducationGradeId_EducationOrientationId",
                schema: "bas",
                table: "EducationGrade_Orientation",
                columns: new[] { "EducationGradeId", "EducationOrientationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EducationGrade_Orientation_EducationOrientationId",
                schema: "bas",
                table: "EducationGrade_Orientation",
                column: "EducationOrientationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Education_Field_Education_Group_EducationGroupId",
                schema: "bas",
                table: "Education_Field",
                column: "EducationGroupId",
                principalSchema: "bas",
                principalTable: "Education_Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Education_Orientation_Education_Field_EducationFieldId",
                schema: "bas",
                table: "Education_Orientation",
                column: "EducationFieldId",
                principalSchema: "bas",
                principalTable: "Education_Field",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

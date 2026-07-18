using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Organisation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AcPosition1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
/* 1) Drop ALL FKs that reference Detail (cross-schema included, e.g. Order.Recruit_Order) */
DECLARE @sql nvarchar(max) = N'';
SELECT @sql += N'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(fk.parent_object_id))
    + N'.' + QUOTENAME(OBJECT_NAME(fk.parent_object_id))
    + N' DROP CONSTRAINT ' + QUOTENAME(fk.name) + N';'
FROM sys.foreign_keys fk
WHERE fk.referenced_object_id = OBJECT_ID(N'Org.Organisation_Position_Detail');
IF LEN(@sql) > 0 EXEC sp_executesql @sql;

/* 2) Drop Detail child tables if present */
IF OBJECT_ID(N'Org.Organisation_Position_Detail_Conversion', N'U') IS NOT NULL
    DROP TABLE [Org].[Organisation_Position_Detail_Conversion];
IF OBJECT_ID(N'Org.Organisation_Position_Detail_History', N'U') IS NOT NULL
    DROP TABLE [Org].[Organisation_Position_Detail_History];
IF OBJECT_ID(N'Org.Organisation_Position_Detail_Job', N'U') IS NOT NULL
    DROP TABLE [Org].[Organisation_Position_Detail_Job];

/* 3) Drop Detail */
IF OBJECT_ID(N'Org.Organisation_Position_Detail', N'U') IS NOT NULL
    DROP TABLE [Org].[Organisation_Position_Detail];

/* 4) Drop FKs referencing Setting, then Setting */
SET @sql = N'';
SELECT @sql += N'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(fk.parent_object_id))
    + N'.' + QUOTENAME(OBJECT_NAME(fk.parent_object_id))
    + N' DROP CONSTRAINT ' + QUOTENAME(fk.name) + N';'
FROM sys.foreign_keys fk
WHERE fk.referenced_object_id = OBJECT_ID(N'Org.Organisation_Position_Setting');
IF LEN(@sql) > 0 EXEC sp_executesql @sql;

IF OBJECT_ID(N'Org.Organisation_Position_Setting', N'U') IS NOT NULL
    DROP TABLE [Org].[Organisation_Position_Setting];

/* 5) Expand Organisation_Position */
IF COL_LENGTH('Org.Organisation_Position', 'Capacity') IS NULL
    ALTER TABLE [Org].[Organisation_Position] ADD [Capacity] int NULL;
IF COL_LENGTH('Org.Organisation_Position', 'InsurancePositionId') IS NULL
    ALTER TABLE [Org].[Organisation_Position] ADD [InsurancePositionId] bigint NOT NULL CONSTRAINT [DF_Organisation_Position_InsurancePositionId] DEFAULT (CAST(0 AS bigint));
IF COL_LENGTH('Org.Organisation_Position', 'IsApproved') IS NULL
    ALTER TABLE [Org].[Organisation_Position] ADD [IsApproved] bit NULL;
IF COL_LENGTH('Org.Organisation_Position', 'IsDedicated') IS NULL
    ALTER TABLE [Org].[Organisation_Position] ADD [IsDedicated] bit NULL;
IF COL_LENGTH('Org.Organisation_Position', 'IsExpert') IS NULL
    ALTER TABLE [Org].[Organisation_Position] ADD [IsExpert] bit NULL;
IF COL_LENGTH('Org.Organisation_Position', 'IsFreez') IS NULL
    ALTER TABLE [Org].[Organisation_Position] ADD [IsFreez] bit NULL;
IF COL_LENGTH('Org.Organisation_Position', 'IsManager') IS NULL
    ALTER TABLE [Org].[Organisation_Position] ADD [IsManager] bit NULL;
IF COL_LENGTH('Org.Organisation_Position', 'IsStarable') IS NULL
    ALTER TABLE [Org].[Organisation_Position] ADD [IsStarable] bit NULL;
IF COL_LENGTH('Org.Organisation_Position', 'IsState') IS NULL
    ALTER TABLE [Org].[Organisation_Position] ADD [IsState] bit NULL;
IF COL_LENGTH('Org.Organisation_Position', 'IsSubstitute') IS NULL
    ALTER TABLE [Org].[Organisation_Position] ADD [IsSubstitute] bit NULL;
IF COL_LENGTH('Org.Organisation_Position', 'LockEndDate') IS NULL
    ALTER TABLE [Org].[Organisation_Position] ADD [LockEndDate] datetime NULL;
IF COL_LENGTH('Org.Organisation_Position', 'LockStartDate') IS NULL
    ALTER TABLE [Org].[Organisation_Position] ADD [LockStartDate] datetime NULL;
IF COL_LENGTH('Org.Organisation_Position', 'PositionCode') IS NULL
    ALTER TABLE [Org].[Organisation_Position] ADD [PositionCode] nvarchar(25) NULL;
IF COL_LENGTH('Org.Organisation_Position', 'RelatedNodeId') IS NULL
    ALTER TABLE [Org].[Organisation_Position] ADD [RelatedNodeId] bigint NOT NULL CONSTRAINT [DF_Organisation_Position_RelatedNodeId] DEFAULT (CAST(0 AS bigint));

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Organisation_Position_InsurancePositionId' AND object_id = OBJECT_ID(N'Org.Organisation_Position'))
    CREATE INDEX [IX_Organisation_Position_InsurancePositionId] ON [Org].[Organisation_Position]([InsurancePositionId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Organisation_Position_RelatedNodeId' AND object_id = OBJECT_ID(N'Org.Organisation_Position'))
    CREATE INDEX [IX_Organisation_Position_RelatedNodeId] ON [Org].[Organisation_Position]([RelatedNodeId]);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Organisation_Position_Insurance_Position_InsurancePositionId')
    ALTER TABLE [Org].[Organisation_Position] WITH CHECK
    ADD CONSTRAINT [FK_Organisation_Position_Insurance_Position_InsurancePositionId]
    FOREIGN KEY ([InsurancePositionId]) REFERENCES [bas].[Insurance_Position]([Id]);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Organisation_Position_Organisation_Chart_RelatedNodeId')
    ALTER TABLE [Org].[Organisation_Position] WITH CHECK
    ADD CONSTRAINT [FK_Organisation_Position_Organisation_Chart_RelatedNodeId]
    FOREIGN KEY ([RelatedNodeId]) REFERENCES [Org].[Organisation_Chart]([Id]);

/* 6) Point Recruit_Order FK to Organisation_Position when column already renamed */
IF OBJECT_ID(N'Order.Recruit_Order', N'U') IS NOT NULL
   AND COL_LENGTH('Order.Recruit_Order', 'OrganisationPositionId') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Recruit_Order_Organisation_Position_OrganisationPositionId')
BEGIN
    ALTER TABLE [Order].[Recruit_Order] WITH CHECK
    ADD CONSTRAINT [FK_Recruit_Order_Organisation_Position_OrganisationPositionId]
    FOREIGN KEY ([OrganisationPositionId]) REFERENCES [Org].[Organisation_Position]([Id]);
END

/* 7) Create Suggested / Supervisor if missing (they do not exist on this DB) */
IF OBJECT_ID(N'Org.Organisation_Position_Suggested', N'U') IS NULL
BEGIN
    CREATE TABLE [Org].[Organisation_Position_Suggested](
        [Id] bigint IDENTITY(1,1) NOT NULL,
        [OrganisationChartId] bigint NOT NULL,
        [EmployeeID] bigint NOT NULL,
        [OrganisationPositionId] bigint NOT NULL,
        [CreatedBy] nvarchar(100) NULL,
        [CreateDate] datetime NULL,
        [LastModifiedDate] datetime NULL,
        [IPAddress] nvarchar(128) NULL,
        [IsDeleted] bit NOT NULL,
        [StartDate] datetime NULL,
        [EndDate] datetime NULL,
        [LastModifiedBy] nvarchar(256) NULL,
        CONSTRAINT [PK_Organisation_Position_Suggested] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Organisation_Position_Suggested_Organisation_Chart_OrganisationChartId]
            FOREIGN KEY ([OrganisationChartId]) REFERENCES [Org].[Organisation_Chart]([Id]),
        CONSTRAINT [FK_Organisation_Position_Suggested_Organisation_Position_OrganisationPositionId]
            FOREIGN KEY ([OrganisationPositionId]) REFERENCES [Org].[Organisation_Position]([Id])
    );
    CREATE INDEX [IX_Organisation_Position_Suggested_OrganisationChartId]
        ON [Org].[Organisation_Position_Suggested]([OrganisationChartId]);
    CREATE INDEX [IX_Organisation_Position_Suggested_OrganisationPositionId]
        ON [Org].[Organisation_Position_Suggested]([OrganisationPositionId]);
END
ELSE
BEGIN
    IF COL_LENGTH('Org.Organisation_Position_Suggested', 'OrganisationPositionDetailId') IS NOT NULL
       AND COL_LENGTH('Org.Organisation_Position_Suggested', 'OrganisationPositionId') IS NULL
        EXEC sp_rename N'Org.Organisation_Position_Suggested.OrganisationPositionDetailId', N'OrganisationPositionId', 'COLUMN';

    IF EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE name = N'IX_Organisation_Position_Suggested_OrganisationPositionDetailId'
          AND object_id = OBJECT_ID(N'Org.Organisation_Position_Suggested'))
    AND NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE name = N'IX_Organisation_Position_Suggested_OrganisationPositionId'
          AND object_id = OBJECT_ID(N'Org.Organisation_Position_Suggested'))
        EXEC sp_rename N'Org.Organisation_Position_Suggested.IX_Organisation_Position_Suggested_OrganisationPositionDetailId',
                        N'IX_Organisation_Position_Suggested_OrganisationPositionId', N'INDEX';

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Organisation_Position_Suggested_Organisation_Position_OrganisationPositionId')
        ALTER TABLE [Org].[Organisation_Position_Suggested] WITH CHECK
        ADD CONSTRAINT [FK_Organisation_Position_Suggested_Organisation_Position_OrganisationPositionId]
        FOREIGN KEY ([OrganisationPositionId]) REFERENCES [Org].[Organisation_Position]([Id]);
END

IF OBJECT_ID(N'Org.Organisation_Position_Supervisor', N'U') IS NULL
BEGIN
    CREATE TABLE [Org].[Organisation_Position_Supervisor](
        [Id] bigint IDENTITY(1,1) NOT NULL,
        [OrganisationChartId] bigint NOT NULL,
        [EmployeeID] bigint NOT NULL,
        [OrganisationPositionId] bigint NOT NULL,
        [CreatedBy] nvarchar(100) NULL,
        [IsMain] bit NOT NULL,
        [CreateDate] datetime NULL,
        [LastModifiedDate] datetime NULL,
        [IPAddress] nvarchar(128) NULL,
        [IsDeleted] bit NOT NULL,
        [StartDate] datetime NULL,
        [EndDate] datetime NULL,
        [LastModifiedBy] nvarchar(256) NULL,
        CONSTRAINT [PK_Organisation_Position_Supervisor] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Organisation_Position_Supervisor_Organisation_Chart_OrganisationChartId]
            FOREIGN KEY ([OrganisationChartId]) REFERENCES [Org].[Organisation_Chart]([Id]),
        CONSTRAINT [FK_Organisation_Position_Supervisor_Organisation_Position_OrganisationPositionId]
            FOREIGN KEY ([OrganisationPositionId]) REFERENCES [Org].[Organisation_Position]([Id])
    );
    CREATE INDEX [IX_Organisation_Position_Supervisor_OrganisationChartId]
        ON [Org].[Organisation_Position_Supervisor]([OrganisationChartId]);
    CREATE INDEX [IX_Organisation_Position_Supervisor_OrganisationPositionId]
        ON [Org].[Organisation_Position_Supervisor]([OrganisationPositionId]);
END
ELSE
BEGIN
    IF COL_LENGTH('Org.Organisation_Position_Supervisor', 'OrganisationPositionDetailId') IS NOT NULL
       AND COL_LENGTH('Org.Organisation_Position_Supervisor', 'OrganisationPositionId') IS NULL
        EXEC sp_rename N'Org.Organisation_Position_Supervisor.OrganisationPositionDetailId', N'OrganisationPositionId', 'COLUMN';

    IF EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE name = N'IX_Organisation_Position_Supervisor_OrganisationPositionDetailId'
          AND object_id = OBJECT_ID(N'Org.Organisation_Position_Supervisor'))
    AND NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE name = N'IX_Organisation_Position_Supervisor_OrganisationPositionId'
          AND object_id = OBJECT_ID(N'Org.Organisation_Position_Supervisor'))
        EXEC sp_rename N'Org.Organisation_Position_Supervisor.IX_Organisation_Position_Supervisor_OrganisationPositionDetailId',
                        N'IX_Organisation_Position_Supervisor_OrganisationPositionId', N'INDEX';

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Organisation_Position_Supervisor_Organisation_Position_OrganisationPositionId')
        ALTER TABLE [Org].[Organisation_Position_Supervisor] WITH CHECK
        ADD CONSTRAINT [FK_Organisation_Position_Supervisor_Organisation_Position_OrganisationPositionId]
        FOREIGN KEY ([OrganisationPositionId]) REFERENCES [Org].[Organisation_Position]([Id]);
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organisation_Position_Insurance_Position_InsurancePositionId",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropForeignKey(
                name: "FK_Organisation_Position_Organisation_Chart_RelatedNodeId",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropForeignKey(
                name: "FK_Organisation_Position_Suggested_Organisation_Position_OrganisationPositionId",
                schema: "Org",
                table: "Organisation_Position_Suggested");

            migrationBuilder.DropForeignKey(
                name: "FK_Organisation_Position_Supervisor_Organisation_Position_OrganisationPositionId",
                schema: "Org",
                table: "Organisation_Position_Supervisor");

            migrationBuilder.DropIndex(
                name: "IX_Organisation_Position_InsurancePositionId",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropIndex(
                name: "IX_Organisation_Position_RelatedNodeId",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropColumn(
                name: "Capacity",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropColumn(
                name: "InsurancePositionId",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropColumn(
                name: "IsDedicated",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropColumn(
                name: "IsExpert",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropColumn(
                name: "IsFreez",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropColumn(
                name: "IsManager",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropColumn(
                name: "IsStarable",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropColumn(
                name: "IsState",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropColumn(
                name: "IsSubstitute",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropColumn(
                name: "LockEndDate",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropColumn(
                name: "LockStartDate",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropColumn(
                name: "PositionCode",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropColumn(
                name: "RelatedNodeId",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.RenameColumn(
                name: "OrganisationPositionId",
                schema: "Org",
                table: "Organisation_Position_Supervisor",
                newName: "OrganisationPositionDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_Organisation_Position_Supervisor_OrganisationPositionId",
                schema: "Org",
                table: "Organisation_Position_Supervisor",
                newName: "IX_Organisation_Position_Supervisor_OrganisationPositionDetailId");

            migrationBuilder.RenameColumn(
                name: "OrganisationPositionId",
                schema: "Org",
                table: "Organisation_Position_Suggested",
                newName: "OrganisationPositionDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_Organisation_Position_Suggested_OrganisationPositionId",
                schema: "Org",
                table: "Organisation_Position_Suggested",
                newName: "IX_Organisation_Position_Suggested_OrganisationPositionDetailId");

            migrationBuilder.AddColumn<long>(
                name: "EducationGroupId",
                schema: "bas",
                table: "Education_Field",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Education_Group",
                schema: "bas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    OldID = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Education_Group", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organisation_Position_Setting",
                schema: "Org",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationChartId = table.Column<long>(type: "bigint", nullable: false),
                    OrganisationPositionId = table.Column<long>(type: "bigint", nullable: false),
                    RelatedNodeId = table.Column<long>(type: "bigint", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisation_Position_Setting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organisation_Position_Setting_Organisation_Chart_OrganisationChartId",
                        column: x => x.OrganisationChartId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Chart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organisation_Position_Setting_Organisation_Chart_RelatedNodeId",
                        column: x => x.RelatedNodeId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Chart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organisation_Position_Setting_Organisation_Position_OrganisationPositionId",
                        column: x => x.OrganisationPositionId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Position",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Organisation_Position_Detail",
                schema: "Org",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsurancePositionId = table.Column<long>(type: "bigint", nullable: false),
                    OrganisationChartId = table.Column<long>(type: "bigint", nullable: false),
                    OrganisationPositionId = table.Column<long>(type: "bigint", nullable: false),
                    OrganisationPositionSettingId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: true),
                    IsDedicated = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsExpert = table.Column<bool>(type: "bit", nullable: true),
                    IsFreez = table.Column<bool>(type: "bit", nullable: true),
                    IsManager = table.Column<bool>(type: "bit", nullable: true),
                    IsStarable = table.Column<bool>(type: "bit", nullable: true),
                    IsState = table.Column<bool>(type: "bit", nullable: true),
                    IsSubstitute = table.Column<bool>(type: "bit", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LockEndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LockStartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    PositionCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisation_Position_Detail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organisation_Position_Detail_Insurance_Position_InsurancePositionId",
                        column: x => x.InsurancePositionId,
                        principalSchema: "bas",
                        principalTable: "Insurance_Position",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organisation_Position_Detail_Organisation_Chart_OrganisationChartId",
                        column: x => x.OrganisationChartId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Chart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organisation_Position_Detail_Organisation_Position_OrganisationPositionId",
                        column: x => x.OrganisationPositionId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Position",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organisation_Position_Detail_Organisation_Position_Setting_OrganisationPositionSettingId",
                        column: x => x.OrganisationPositionSettingId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Position_Setting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Organisation_Position_Detail_Conversion",
                schema: "Org",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromOrganisationPositionDetailId = table.Column<long>(type: "bigint", nullable: true),
                    OrganisationChartId = table.Column<long>(type: "bigint", nullable: false),
                    ToOrganisationPositionDetailId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsLast = table.Column<bool>(type: "bit", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisation_Position_Detail_Conversion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organisation_Position_Detail_Conversion_Organisation_Chart_OrganisationChartId",
                        column: x => x.OrganisationChartId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Chart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organisation_Position_Detail_Conversion_Organisation_Position_Detail_FromOrganisationPositionDetailId",
                        column: x => x.FromOrganisationPositionDetailId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Position_Detail",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Organisation_Position_Detail_Conversion_Organisation_Position_Detail_ToOrganisationPositionDetailId",
                        column: x => x.ToOrganisationPositionDetailId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Position_Detail",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Organisation_Position_Detail_History",
                schema: "Org",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationChartId = table.Column<long>(type: "bigint", nullable: false),
                    OrganisationPositionDetailId = table.Column<long>(type: "bigint", nullable: false),
                    OrganisationPositionId = table.Column<long>(type: "bigint", nullable: false),
                    OrganisationPositionSettingId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: true),
                    IsCastedInRec = table.Column<bool>(type: "bit", nullable: true),
                    IsDedicated = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsStarable = table.Column<bool>(type: "bit", nullable: true),
                    IsState = table.Column<bool>(type: "bit", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LockEndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LockStartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    PositionCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisation_Position_Detail_History", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organisation_Position_Detail_History_Organisation_Chart_OrganisationChartId",
                        column: x => x.OrganisationChartId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Chart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organisation_Position_Detail_History_Organisation_Position_Detail_OrganisationPositionDetailId",
                        column: x => x.OrganisationPositionDetailId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Position_Detail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organisation_Position_Detail_History_Organisation_Position_OrganisationPositionId",
                        column: x => x.OrganisationPositionId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Position",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organisation_Position_Detail_History_Organisation_Position_Setting_OrganisationPositionSettingId",
                        column: x => x.OrganisationPositionSettingId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Position_Setting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Organisation_Position_Detail_Job",
                schema: "Org",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationChartId = table.Column<long>(type: "bigint", nullable: false),
                    OrganisationPositionDetailId = table.Column<long>(type: "bigint", nullable: false),
                    OrganizationJobId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisation_Position_Detail_Job", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organisation_Position_Detail_Job_Organisation_Chart_OrganisationChartId",
                        column: x => x.OrganisationChartId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Chart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organisation_Position_Detail_Job_Organisation_Job_OrganizationJobId",
                        column: x => x.OrganizationJobId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organisation_Position_Detail_Job_Organisation_Position_Detail_OrganisationPositionDetailId",
                        column: x => x.OrganisationPositionDetailId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Position_Detail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Education_Field_EducationGroupId",
                schema: "bas",
                table: "Education_Field",
                column: "EducationGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Position_Detail_InsurancePositionId",
                schema: "Org",
                table: "Organisation_Position_Detail",
                column: "InsurancePositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Position_Detail_OrganisationChartId",
                schema: "Org",
                table: "Organisation_Position_Detail",
                column: "OrganisationChartId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Position_Detail_OrganisationPositionId",
                schema: "Org",
                table: "Organisation_Position_Detail",
                column: "OrganisationPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Position_Detail_OrganisationPositionSettingId",
                schema: "Org",
                table: "Organisation_Position_Detail",
                column: "OrganisationPositionSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Position_Detail_Conversion_FromOrganisationPositionDetailId",
                schema: "Org",
                table: "Organisation_Position_Detail_Conversion",
                column: "FromOrganisationPositionDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Position_Detail_Conversion_OrganisationChartId",
                schema: "Org",
                table: "Organisation_Position_Detail_Conversion",
                column: "OrganisationChartId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Position_Detail_Conversion_ToOrganisationPositionDetailId",
                schema: "Org",
                table: "Organisation_Position_Detail_Conversion",
                column: "ToOrganisationPositionDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Position_Detail_History_OrganisationChartId",
                schema: "Org",
                table: "Organisation_Position_Detail_History",
                column: "OrganisationChartId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Position_Detail_History_OrganisationPositionDetailId",
                schema: "Org",
                table: "Organisation_Position_Detail_History",
                column: "OrganisationPositionDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Position_Detail_History_OrganisationPositionId",
                schema: "Org",
                table: "Organisation_Position_Detail_History",
                column: "OrganisationPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Position_Detail_History_OrganisationPositionSettingId",
                schema: "Org",
                table: "Organisation_Position_Detail_History",
                column: "OrganisationPositionSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Position_Detail_Job_OrganisationChartId",
                schema: "Org",
                table: "Organisation_Position_Detail_Job",
                column: "OrganisationChartId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Position_Detail_Job_OrganisationPositionDetailId",
                schema: "Org",
                table: "Organisation_Position_Detail_Job",
                column: "OrganisationPositionDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Position_Detail_Job_OrganizationJobId",
                schema: "Org",
                table: "Organisation_Position_Detail_Job",
                column: "OrganizationJobId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Position_Setting_OrganisationChartId",
                schema: "Org",
                table: "Organisation_Position_Setting",
                column: "OrganisationChartId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Position_Setting_OrganisationPositionId",
                schema: "Org",
                table: "Organisation_Position_Setting",
                column: "OrganisationPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Position_Setting_RelatedNodeId",
                schema: "Org",
                table: "Organisation_Position_Setting",
                column: "RelatedNodeId");

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
                name: "FK_Organisation_Position_Suggested_Organisation_Position_Detail_OrganisationPositionDetailId",
                schema: "Org",
                table: "Organisation_Position_Suggested",
                column: "OrganisationPositionDetailId",
                principalSchema: "Org",
                principalTable: "Organisation_Position_Detail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Organisation_Position_Supervisor_Organisation_Position_Detail_OrganisationPositionDetailId",
                schema: "Org",
                table: "Organisation_Position_Supervisor",
                column: "OrganisationPositionDetailId",
                principalSchema: "Org",
                principalTable: "Organisation_Position_Detail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

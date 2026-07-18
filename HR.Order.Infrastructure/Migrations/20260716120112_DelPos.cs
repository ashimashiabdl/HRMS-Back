using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Order.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DelPos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropForeignKey(
                name: "FK_Recruit_Order_Organisation_Position_Detail_OrganisationPositionDetailId",
                schema: "Order",
                table: "Recruit_Order");


            migrationBuilder.RenameColumn(
                name: "OrganisationPositionDetailId",
                schema: "Order",
                table: "Recruit_Order",
                newName: "OrganisationPositionId");

            migrationBuilder.RenameIndex(
                name: "IX_Recruit_Order_OrganisationPositionDetailId",
                schema: "Order",
                table: "Recruit_Order",
                newName: "IX_Recruit_Order_OrganisationPositionId");


            migrationBuilder.AddForeignKey(
                name: "FK_Recruit_Order_Organisation_Position_OrganisationPositionId",
                schema: "Order",
                table: "Recruit_Order",
                column: "OrganisationPositionId",
                principalSchema: "Org",
                principalTable: "Organisation_Position",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Education_Education_Group_EducationGroupId",
                schema: "emp",
                table: "Education");

            migrationBuilder.DropForeignKey(
                name: "FK_Organisation_Position_Insurance_Position_InsurancePositionId",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropForeignKey(
                name: "FK_Organisation_Position_Organisation_Chart_RelatedNodeId",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropForeignKey(
                name: "FK_Recruit_Order_Organisation_Position_OrganisationPositionId",
                schema: "Order",
                table: "Recruit_Order");

            migrationBuilder.DropIndex(
                name: "IX_Organisation_Position_InsurancePositionId",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropIndex(
                name: "IX_Organisation_Position_RelatedNodeId",
                schema: "Org",
                table: "Organisation_Position");

            migrationBuilder.DropIndex(
                name: "IX_Education_EducationGroupId",
                schema: "emp",
                table: "Education");

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

            migrationBuilder.DropColumn(
                name: "JobFinancialCode",
                schema: "Org",
                table: "Organisation_Job");

            migrationBuilder.DropColumn(
                name: "DeathCause",
                schema: "emp",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "DeathDate",
                schema: "emp",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "EnglishFirstName",
                schema: "emp",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "EnglishLastName",
                schema: "emp",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "IsAuthorizedForeigner",
                schema: "emp",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "EducationGroupId",
                schema: "emp",
                table: "Education");

            migrationBuilder.RenameColumn(
                name: "OrganisationPositionId",
                schema: "Order",
                table: "Recruit_Order",
                newName: "OrganisationPositionDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_Recruit_Order_OrganisationPositionId",
                schema: "Order",
                table: "Recruit_Order",
                newName: "IX_Recruit_Order_OrganisationPositionDetailId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Education_Orientation_EducationFieldId",
                schema: "bas",
                table: "Education_Orientation",
                column: "EducationFieldId");

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
                name: "FK_Education_Orientation_Education_Field_EducationFieldId",
                schema: "bas",
                table: "Education_Orientation",
                column: "EducationFieldId",
                principalSchema: "bas",
                principalTable: "Education_Field",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Recruit_Order_Organisation_Position_Detail_OrganisationPositionDetailId",
                schema: "Order",
                table: "Recruit_Order",
                column: "OrganisationPositionDetailId",
                principalSchema: "Org",
                principalTable: "Organisation_Position_Detail",
                principalColumn: "Id");
        }
    }
}

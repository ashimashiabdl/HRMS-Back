using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleASgfgfnnnnnsol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Role_Report",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationChartId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    DynamicReportId = table.Column<long>(type: "bigint", nullable: false, comment: "fill from DynamicReport Table in schema rpt"),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role_Report", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Role_Report_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Role_Report_Organisation_Chart_OrganisationChartId",
                        column: x => x.OrganisationChartId,
                        principalSchema: "Org",
                        principalTable: "Organisation_Chart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Role_Report_OrganisationChartId",
                schema: "Identity",
                table: "Role_Report",
                column: "OrganisationChartId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Report_RoleId",
                schema: "Identity",
                table: "Role_Report",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Role_Report",
                schema: "Identity");
        }
    }
}

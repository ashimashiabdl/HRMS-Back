using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.FormulaEngine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Sesds343434fero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Formula_Definition_History",
                schema: "For",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormulaDefinitionId = table.Column<long>(type: "bigint", nullable: false),
                    PreviousFormulaText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    UserFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangeDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formula_Definition_History", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Formula_Definition_History_Formula_Definition_FormulaDefinitionId",
                        column: x => x.FormulaDefinitionId,
                        principalSchema: "For",
                        principalTable: "Formula_Definition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Formula_Definition_History_FormulaDefinitionId",
                schema: "For",
                table: "Formula_Definition_History",
                column: "FormulaDefinitionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Formula_Definition_History",
                schema: "For");
        }
    }
}

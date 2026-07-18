using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Employee.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Englishname3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // bas.Education_Field / Education_Orientation column removals are owned by
            // BaseInfo migration ExcelDB6 — do not drop them here.

            migrationBuilder.AddColumn<long>(
                name: "EducationGroupId",
                schema: "emp",
                table: "Education",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Education_EducationGroupId",
                schema: "emp",
                table: "Education",
                column: "EducationGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Education_Education_Group_EducationGroupId",
                schema: "emp",
                table: "Education",
                column: "EducationGroupId",
                principalSchema: "bas",
                principalTable: "Education_Group",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Education_Education_Group_EducationGroupId",
                schema: "emp",
                table: "Education");

            migrationBuilder.DropIndex(
                name: "IX_Education_EducationGroupId",
                schema: "emp",
                table: "Education");

            migrationBuilder.DropColumn(
                name: "EducationGroupId",
                schema: "emp",
                table: "Education");
        }
    }
}

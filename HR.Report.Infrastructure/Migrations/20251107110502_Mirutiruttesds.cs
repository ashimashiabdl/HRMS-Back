using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Report.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Mirutiruttesds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Field_Operator_Field_DataType_FieldDataTypeId1",
                schema: "rpt",
                table: "Field_Operator");

            migrationBuilder.DropForeignKey(
                name: "FK_Reportable_Field_Field_DataType_FieldDataTypeId1",
                schema: "rpt",
                table: "Reportable_Field");

            migrationBuilder.DropIndex(
                name: "IX_Reportable_Field_FieldDataTypeId1",
                schema: "rpt",
                table: "Reportable_Field");

            migrationBuilder.DropIndex(
                name: "IX_Field_Operator_FieldDataTypeId1",
                schema: "rpt",
                table: "Field_Operator");

            migrationBuilder.DropColumn(
                name: "FieldDataTypeId1",
                schema: "rpt",
                table: "Reportable_Field");

            migrationBuilder.DropColumn(
                name: "FieldDataTypeId1",
                schema: "rpt",
                table: "Field_Operator");

            migrationBuilder.AlterColumn<long>(
                name: "FieldDataTypeId",
                schema: "rpt",
                table: "Reportable_Field",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "FieldDataTypeId",
                schema: "rpt",
                table: "Field_Operator",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Reportable_Field_FieldDataTypeId",
                schema: "rpt",
                table: "Reportable_Field",
                column: "FieldDataTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Field_Operator_FieldDataTypeId",
                schema: "rpt",
                table: "Field_Operator",
                column: "FieldDataTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Field_Operator_Field_DataType_FieldDataTypeId",
                schema: "rpt",
                table: "Field_Operator",
                column: "FieldDataTypeId",
                principalSchema: "rpt",
                principalTable: "Field_DataType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reportable_Field_Field_DataType_FieldDataTypeId",
                schema: "rpt",
                table: "Reportable_Field",
                column: "FieldDataTypeId",
                principalSchema: "rpt",
                principalTable: "Field_DataType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Field_Operator_Field_DataType_FieldDataTypeId",
                schema: "rpt",
                table: "Field_Operator");

            migrationBuilder.DropForeignKey(
                name: "FK_Reportable_Field_Field_DataType_FieldDataTypeId",
                schema: "rpt",
                table: "Reportable_Field");

            migrationBuilder.DropIndex(
                name: "IX_Reportable_Field_FieldDataTypeId",
                schema: "rpt",
                table: "Reportable_Field");

            migrationBuilder.DropIndex(
                name: "IX_Field_Operator_FieldDataTypeId",
                schema: "rpt",
                table: "Field_Operator");

            migrationBuilder.AlterColumn<int>(
                name: "FieldDataTypeId",
                schema: "rpt",
                table: "Reportable_Field",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "FieldDataTypeId1",
                schema: "rpt",
                table: "Reportable_Field",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<int>(
                name: "FieldDataTypeId",
                schema: "rpt",
                table: "Field_Operator",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "FieldDataTypeId1",
                schema: "rpt",
                table: "Field_Operator",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Reportable_Field_FieldDataTypeId1",
                schema: "rpt",
                table: "Reportable_Field",
                column: "FieldDataTypeId1");

            migrationBuilder.CreateIndex(
                name: "IX_Field_Operator_FieldDataTypeId1",
                schema: "rpt",
                table: "Field_Operator",
                column: "FieldDataTypeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Field_Operator_Field_DataType_FieldDataTypeId1",
                schema: "rpt",
                table: "Field_Operator",
                column: "FieldDataTypeId1",
                principalSchema: "rpt",
                principalTable: "Field_DataType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reportable_Field_Field_DataType_FieldDataTypeId1",
                schema: "rpt",
                table: "Reportable_Field",
                column: "FieldDataTypeId1",
                principalSchema: "rpt",
                principalTable: "Field_DataType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

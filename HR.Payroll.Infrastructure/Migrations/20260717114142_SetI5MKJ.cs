using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Payroll.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetI5MKJ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // FKهای موجود Arear_Fiche / Arear_FicheItem را Drop نکنید:
            // نام constraint در دیتابیس با نام EF یکسان نیست و باعث خطای 3728 می‌شود.

            migrationBuilder.AddColumn<long>(
                name: "ArearPaymentPeriodId",
                schema: "Payroll",
                table: "Arear_FicheItem",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EmployeeDeductionId",
                schema: "Payroll",
                table: "Arear_FicheItem",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EmployeeFundId",
                schema: "Payroll",
                table: "Arear_FicheItem",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FundSumAmount",
                schema: "Payroll",
                table: "Arear_FicheItem",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArear",
                schema: "Payroll",
                table: "Arear_FicheItem",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: " آیا این قلم منشا معوقه دارد ؟");

            migrationBuilder.AddColumn<bool>(
                name: "IsEmployerItem",
                schema: "Payroll",
                table: "Arear_FicheItem",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSubItem",
                schema: "Payroll",
                table: "Arear_FicheItem",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "قلم فرعی می باشد");

            migrationBuilder.AddColumn<long>(
                name: "PersonnelLoanId",
                schema: "Payroll",
                table: "Arear_FicheItem",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PersonnelPaymentId",
                schema: "Payroll",
                table: "Arear_FicheItem",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RemainDeductionAmount",
                schema: "Payroll",
                table: "Arear_FicheItem",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RemainLoanAmount",
                schema: "Payroll",
                table: "Arear_FicheItem",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ArearId",
                schema: "Payroll",
                table: "Arear_Fiche",
                type: "bigint",
                nullable: true,
                comment: "معوقه والد");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "Payroll",
                table: "Arear_Fiche",
                type: "nvarchar(1500)",
                maxLength: 1500,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IncAmount",
                schema: "Payroll",
                table: "Arear_Fiche",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "InsuranceTotal_DSW",
                schema: "Payroll",
                table: "Arear_Fiche",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "جمع کل دستمزد و مزایای ماهانه برای دیسکت بیمه (بدون اقلام حق اولاد)");

            migrationBuilder.AddColumn<long>(
                name: "SpouseAmount",
                schema: "Payroll",
                table: "Arear_Fiche",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ApproveTimePaymentPeriodId",
                schema: "Payroll",
                table: "Arear",
                type: "bigint",
                nullable: true,
                comment: "دوره حقوقی زمان تایید حکم");

            migrationBuilder.AddColumn<int>(
                name: "ArearFicheCount",
                schema: "Payroll",
                table: "Arear",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "تعداد فیش معوقه");

            migrationBuilder.AddColumn<long>(
                name: "ArearsStatusId",
                schema: "Payroll",
                table: "Arear",
                type: "bigint",
                nullable: false,
                defaultValue: 4L,
                comment: "وضعیت معوقه");

            migrationBuilder.AddColumn<DateTime>(
                name: "CalculatedDate",
                schema: "Payroll",
                table: "Arear",
                type: "datetime",
                nullable: true,
                comment: "زمان محاسبه");

            migrationBuilder.AddColumn<int>(
                name: "ChangedItemCount",
                schema: "Payroll",
                table: "Arear",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "تعداد اقلام تغییر یافته");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "Payroll",
                table: "Arear",
                type: "nvarchar(1500)",
                maxLength: 1500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastErrorMessage",
                schema: "Payroll",
                table: "Arear",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                comment: "آخرین پیام خطا یا علت رد");

            migrationBuilder.AddColumn<long>(
                name: "PayableDifferenceAmount",
                schema: "Payroll",
                table: "Arear",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "خالص تفاوت قابل پرداخت");

            migrationBuilder.AddColumn<long>(
                name: "PaymentPeriodIntendToPayId",
                schema: "Payroll",
                table: "Arear",
                type: "bigint",
                nullable: true,
                comment: "دوره قصد پرداخت معوقه");

            migrationBuilder.AddColumn<long>(
                name: "TotalDifferenceAmount",
                schema: "Payroll",
                table: "Arear",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "جمع تفاوت اقلام تغییر یافته");



            migrationBuilder.CreateIndex(
                name: "IX_Arear_FicheItem_ArearPaymentPeriodId",
                schema: "Payroll",
                table: "Arear_FicheItem",
                column: "ArearPaymentPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Arear_FicheItem_EmployeeDeductionId",
                schema: "Payroll",
                table: "Arear_FicheItem",
                column: "EmployeeDeductionId");

            migrationBuilder.CreateIndex(
                name: "IX_Arear_FicheItem_EmployeeFundId",
                schema: "Payroll",
                table: "Arear_FicheItem",
                column: "EmployeeFundId");

            migrationBuilder.CreateIndex(
                name: "IX_Arear_FicheItem_PersonnelLoanId",
                schema: "Payroll",
                table: "Arear_FicheItem",
                column: "PersonnelLoanId");

            migrationBuilder.CreateIndex(
                name: "IX_Arear_FicheItem_PersonnelPaymentId",
                schema: "Payroll",
                table: "Arear_FicheItem",
                column: "PersonnelPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Arear_Fiche_ArearId",
                schema: "Payroll",
                table: "Arear_Fiche",
                column: "ArearId");

            migrationBuilder.CreateIndex(
                name: "IX_Arear_ApproveTimePaymentPeriodId",
                schema: "Payroll",
                table: "Arear",
                column: "ApproveTimePaymentPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Arear_ArearsStatusId",
                schema: "Payroll",
                table: "Arear",
                column: "ArearsStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Arear_PaymentPeriodIntendToPayId",
                schema: "Payroll",
                table: "Arear",
                column: "PaymentPeriodIntendToPayId");

            migrationBuilder.AddForeignKey(
                name: "FK_Arear_Arears_Status_ArearsStatusId",
                schema: "Payroll",
                table: "Arear",
                column: "ArearsStatusId",
                principalSchema: "Payroll",
                principalTable: "Arears_Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Arear_Payment_Period_ApproveTimePaymentPeriodId",
                schema: "Payroll",
                table: "Arear",
                column: "ApproveTimePaymentPeriodId",
                principalSchema: "Payroll",
                principalTable: "Payment_Period",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Arear_Payment_Period_PaymentPeriodIntendToPayId",
                schema: "Payroll",
                table: "Arear",
                column: "PaymentPeriodIntendToPayId",
                principalSchema: "Payroll",
                principalTable: "Payment_Period",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Arear_Fiche_Arear_ArearId",
                schema: "Payroll",
                table: "Arear_Fiche",
                column: "ArearId",
                principalSchema: "Payroll",
                principalTable: "Arear",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Arear_FicheItem_Employee_Deduction_EmployeeDeductionId",
                schema: "Payroll",
                table: "Arear_FicheItem",
                column: "EmployeeDeductionId",
                principalSchema: "Payroll",
                principalTable: "Employee_Deduction",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Arear_FicheItem_Employee_Fund_EmployeeFundId",
                schema: "Payroll",
                table: "Arear_FicheItem",
                column: "EmployeeFundId",
                principalSchema: "Payroll",
                principalTable: "Employee_Fund",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Arear_FicheItem_Payment_Period_ArearPaymentPeriodId",
                schema: "Payroll",
                table: "Arear_FicheItem",
                column: "ArearPaymentPeriodId",
                principalSchema: "Payroll",
                principalTable: "Payment_Period",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Arear_FicheItem_Personnel_Loan_PersonnelLoanId",
                schema: "Payroll",
                table: "Arear_FicheItem",
                column: "PersonnelLoanId",
                principalSchema: "Payroll",
                principalTable: "Personnel_Loan",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Arear_FicheItem_Personnel_Payment_PersonnelPaymentId",
                schema: "Payroll",
                table: "Arear_FicheItem",
                column: "PersonnelPaymentId",
                principalSchema: "Payroll",
                principalTable: "Personnel_Payment",
                principalColumn: "Id");
        }

           

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Arear_Arears_Status_ArearsStatusId",
                schema: "Payroll",
                table: "Arear");

            migrationBuilder.DropForeignKey(
                name: "FK_Arear_Payment_Period_ApproveTimePaymentPeriodId",
                schema: "Payroll",
                table: "Arear");

            migrationBuilder.DropForeignKey(
                name: "FK_Arear_Payment_Period_PaymentPeriodIntendToPayId",
                schema: "Payroll",
                table: "Arear");

            migrationBuilder.DropForeignKey(
                name: "FK_Arear_Fiche_Arear_ArearId",
                schema: "Payroll",
                table: "Arear_Fiche");

            migrationBuilder.DropForeignKey(
                name: "FK_Arear_FicheItem_Employee_Deduction_EmployeeDeductionId",
                schema: "Payroll",
                table: "Arear_FicheItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Arear_FicheItem_Employee_Fund_EmployeeFundId",
                schema: "Payroll",
                table: "Arear_FicheItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Arear_FicheItem_Payment_Period_ArearPaymentPeriodId",
                schema: "Payroll",
                table: "Arear_FicheItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Arear_FicheItem_Personnel_Loan_PersonnelLoanId",
                schema: "Payroll",
                table: "Arear_FicheItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Arear_FicheItem_Personnel_Payment_PersonnelPaymentId",
                schema: "Payroll",
                table: "Arear_FicheItem");

            migrationBuilder.DropIndex(
                name: "IX_Arear_FicheItem_ArearPaymentPeriodId",
                schema: "Payroll",
                table: "Arear_FicheItem");

            migrationBuilder.DropIndex(
                name: "IX_Arear_FicheItem_EmployeeDeductionId",
                schema: "Payroll",
                table: "Arear_FicheItem");

            migrationBuilder.DropIndex(
                name: "IX_Arear_FicheItem_EmployeeFundId",
                schema: "Payroll",
                table: "Arear_FicheItem");

            migrationBuilder.DropIndex(
                name: "IX_Arear_FicheItem_PersonnelLoanId",
                schema: "Payroll",
                table: "Arear_FicheItem");

            migrationBuilder.DropIndex(
                name: "IX_Arear_FicheItem_PersonnelPaymentId",
                schema: "Payroll",
                table: "Arear_FicheItem");

            migrationBuilder.DropIndex(
                name: "IX_Arear_Fiche_ArearId",
                schema: "Payroll",
                table: "Arear_Fiche");

            migrationBuilder.DropIndex(
                name: "IX_Arear_ApproveTimePaymentPeriodId",
                schema: "Payroll",
                table: "Arear");

            migrationBuilder.DropIndex(
                name: "IX_Arear_ArearsStatusId",
                schema: "Payroll",
                table: "Arear");

            migrationBuilder.DropIndex(
                name: "IX_Arear_PaymentPeriodIntendToPayId",
                schema: "Payroll",
                table: "Arear");

            migrationBuilder.DropColumn(
                name: "ArearPaymentPeriodId",
                schema: "Payroll",
                table: "Arear_FicheItem");

            migrationBuilder.DropColumn(
                name: "EmployeeDeductionId",
                schema: "Payroll",
                table: "Arear_FicheItem");

            migrationBuilder.DropColumn(
                name: "EmployeeFundId",
                schema: "Payroll",
                table: "Arear_FicheItem");

            migrationBuilder.DropColumn(
                name: "FundSumAmount",
                schema: "Payroll",
                table: "Arear_FicheItem");

            migrationBuilder.DropColumn(
                name: "IsArear",
                schema: "Payroll",
                table: "Arear_FicheItem");

            migrationBuilder.DropColumn(
                name: "IsEmployerItem",
                schema: "Payroll",
                table: "Arear_FicheItem");

            migrationBuilder.DropColumn(
                name: "IsSubItem",
                schema: "Payroll",
                table: "Arear_FicheItem");

            migrationBuilder.DropColumn(
                name: "PersonnelLoanId",
                schema: "Payroll",
                table: "Arear_FicheItem");

            migrationBuilder.DropColumn(
                name: "PersonnelPaymentId",
                schema: "Payroll",
                table: "Arear_FicheItem");

            migrationBuilder.DropColumn(
                name: "RemainDeductionAmount",
                schema: "Payroll",
                table: "Arear_FicheItem");

            migrationBuilder.DropColumn(
                name: "RemainLoanAmount",
                schema: "Payroll",
                table: "Arear_FicheItem");

            migrationBuilder.DropColumn(
                name: "ArearId",
                schema: "Payroll",
                table: "Arear_Fiche");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "Payroll",
                table: "Arear_Fiche");

            migrationBuilder.DropColumn(
                name: "IncAmount",
                schema: "Payroll",
                table: "Arear_Fiche");

            migrationBuilder.DropColumn(
                name: "InsuranceTotal_DSW",
                schema: "Payroll",
                table: "Arear_Fiche");

            migrationBuilder.DropColumn(
                name: "SpouseAmount",
                schema: "Payroll",
                table: "Arear_Fiche");

            migrationBuilder.DropColumn(
                name: "ApproveTimePaymentPeriodId",
                schema: "Payroll",
                table: "Arear");

            migrationBuilder.DropColumn(
                name: "ArearFicheCount",
                schema: "Payroll",
                table: "Arear");

            migrationBuilder.DropColumn(
                name: "ArearsStatusId",
                schema: "Payroll",
                table: "Arear");

            migrationBuilder.DropColumn(
                name: "CalculatedDate",
                schema: "Payroll",
                table: "Arear");

            migrationBuilder.DropColumn(
                name: "ChangedItemCount",
                schema: "Payroll",
                table: "Arear");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "Payroll",
                table: "Arear");

            migrationBuilder.DropColumn(
                name: "LastErrorMessage",
                schema: "Payroll",
                table: "Arear");

            migrationBuilder.DropColumn(
                name: "PayableDifferenceAmount",
                schema: "Payroll",
                table: "Arear");

            migrationBuilder.DropColumn(
                name: "PaymentPeriodIntendToPayId",
                schema: "Payroll",
                table: "Arear");

            migrationBuilder.DropColumn(
                name: "TotalDifferenceAmount",
                schema: "Payroll",
                table: "Arear");
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Arear_Fiche", Schema = "Payroll")]
public partial class ArearFiche
{
    [Key]
    public long Id { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string? Ipaddress { get; set; }

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    public long OrganisationChartId { get; set; }

    public long PersonnelFunctionId { get; set; }

    public long InterdictOrderId { get; set; }

    public long PaymentPeriodId { get; set; }

    public long FicheStatusId { get; set; }

    public long EmployeeId { get; set; }

    public long EmployeeTypeId { get; set; }

    public long CostCenterId { get; set; }

    public long? PeymanRowId { get; set; }

    public long DeductedAmount { get; set; }

    public long TotalAmount { get; set; }

    public long PurePaymentAmount { get; set; }

    public long PaymentTax { get; set; }

    public long PaymentInsuranceCovered { get; set; }

    public long PaymentRetiredCovered { get; set; }

    public long? DailyFunctionAmount { get; set; }

    public long? UnEmploymentAmount { get; set; }

    public bool? IsActiveInsurance { get; set; }

    public long MonthJobBenefit { get; set; }

    public long? BillBazkharidSanavatAmount { get; set; }

    public long? BillEydiOpadashAmount { get; set; }

    public long? BillSumItemsAmount { get; set; }

    public long? BillItemsWage { get; set; }

    public long? SumCashTaxCoveredAndCountinious { get; set; }

    public long? SumNonCashTaxCoveredAndCountinious { get; set; }

    public long? SumNonCashTaxCoveredAndNotCountinious { get; set; }

    public long? SumCashTaxCoveredAndNotCountinious { get; set; }

    public long NotNetCurrentMonthOverTimePayment { get; set; }

    public long SumOfDelayedCountiniousPaymentInCurrentMonth { get; set; }

    public long HouseAmount { get; set; }

    public long CarAmount { get; set; }

    [StringLength(128)]
    public string? BankAccountNo { get; set; }

    [StringLength(64)]
    public string? InsuranceNo { get; set; }

    public long PaymentPeriodIntendToPayId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }
}

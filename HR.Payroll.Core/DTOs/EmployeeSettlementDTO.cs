using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HR.Payroll.Core.DTOs;

/// <summary>
/// تسویه حساب کارکنان
/// </summary>
public class EmployeeSettlementDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }
    public string? OrganisationChartTitle { get; set; }
    public long? EmployeeTypeId { get; set; }
    public string? EmployeeTypeTitle { get; set; }
    public long EmployeeId { get; set; }
    public string? EmployeeTitle { get; set; }

    [Required(ErrorMessage = "تاریخ تسویه حساب الزامی است")]
    public DateTime SettlementDate { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public long SettlementCauseId { get; set; }
    public string? SettlementCauseTitle { get; set; }
    public long? SettlementStatusId { get; set; }
    public string? SettlementStatusTitle { get; set; }
    public long InterdictOrderId { get; set; }
    public string? InterdictOrderTitle { get; set; }
    public long LastInterdictOrderId { get; set; }
    public string? LastInterdictOrderTitle { get; set; }
    public long? FicheId { get; set; }
    public string? FicheTitle { get; set; }

    public int FiscalYear { get; set; }

    [StringLength(1024)]
    public string? Description { get; set; }

    [StringLength(6)]
    public string? Duration { get; set; }

    public long PaymentAmount { get; set; }
    public long PurePaymentAmount { get; set; }
    public long DeductionSum { get; set; }

    [StringLength(128)]
    public string? BankAccountNo { get; set; }

    public bool IsYearLong { get; set; }

    /// <summary>
    /// آیا وام‌ها در تسویه حساب لحاظ می‌شوند؟
    /// </summary>
    public bool Loanincluded { get; set; }

    /// <summary>
    /// آیا کسورات در تسویه حساب لحاظ می‌شوند؟
    /// </summary>
    public bool Deductionincluded { get; set; }

    /// <summary>
    /// کاربر تأیید کرده که بدون فیش حقوقی در بازه انتخابی ادامه دهد.
    /// </summary>
    public bool ProceedWithoutFiche { get; set; }

    public List<long>? SettlementItemIds { get; set; }

    public List<EmployeeSettlementItemDTO>? SettlementItems { get; set; }
}

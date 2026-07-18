using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HR.Payroll.Core.DTOs;

public class FicheItemDTO : BaseDTO
{
    public long FicheId { get; set; }
    public long WageItemId { get; set; }
    public string? WageItem { get; set; }
    public long PaymentTypeId { get; set; }
    public string? PaymentType { get; set; }
    public double Value { get; set; }
    public string? Comment { get; set; }
    public long? RemainLoanAmount { get; set; }
    public long? RemainDeductionAmount { get; set; }
    public long? PersonnelLoanId { get; set; }
    public string? PersonnelLoan { get; set; }
    public string? PaymentPeriod { get; set; }
    /// <summary>
    /// آیا این قلم منشا معوقه دارد ؟
    /// </summary>
    [Comment(" آیا این قلم منشا معوقه دارد ؟")]
    public bool IsArear { get; set; }
    public long? ArearPaymentPeriodId { get; set; }
    public string? ArearPaymentPeriod { get; set; }
    public bool? IsEmployerItem { get; set; }
    /// <summary>
    /// قلم فرعی می باشد
    /// </summary>
    [Comment("قلم فرعی می باشد")]
    public bool IsSubItem { get; set; }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Employee_Deduction", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("DeductionTypeId", Name = "IX_Employee_Deduction_DeductionTypeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeDeductionUploadBatchId", Name = "IX_Employee_Deduction_EmployeeDeductionUploadBatchId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Employee_Deduction_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Employee_Deduction_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("StartDeductPaymentPeriodId", Name = "IX_Employee_Deduction_StartDeductPaymentPeriodId")]
[Microsoft.EntityFrameworkCore.Index("TempEmployeeDeductionId", Name = "IX_Employee_Deduction_TempEmployeeDeductionId")]
public partial class EmployeeDeduction
{
    [Key]
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public long DeductionTypeId { get; set; }

    public long OrganisationChartId { get; set; }

    public long StartDeductPaymentPeriodId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime PaymentDate { get; set; }

    [StringLength(128)]
    public string? LoanPaymentDocDesc { get; set; }

    public long? AllAmount { get; set; }

    public long? InstallmentAmount { get; set; }

    public bool RemainingCrumbsAtFirst { get; set; }

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

    public bool IsActive { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    public long? EmployeeDeductionUploadBatchId { get; set; }

    public long? FileId { get; set; }

    public long? TempEmployeeDeductionId { get; set; }

    [ForeignKey("DeductionTypeId")]
    [InverseProperty("EmployeeDeductions")]
    public virtual DeductionType DeductionType { get; set; } = null!;

    [ForeignKey("EmployeeId")]
    [InverseProperty("EmployeeDeductions")]
    public virtual Employee Employee { get; set; } = null!;

    [InverseProperty("EmployeeDeduction")]
    public virtual ICollection<EmployeeDeductionPayment> EmployeeDeductionPayments { get; set; } = new List<EmployeeDeductionPayment>();

    [ForeignKey("EmployeeDeductionUploadBatchId")]
    [InverseProperty("EmployeeDeductions")]
    public virtual EmployeeDeductionUploadBatch? EmployeeDeductionUploadBatch { get; set; }

    [InverseProperty("EmployeeDeduction")]
    public virtual ICollection<FicheItem> FicheItems { get; set; } = new List<FicheItem>();

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("EmployeeDeductions")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("StartDeductPaymentPeriodId")]
    [InverseProperty("EmployeeDeductions")]
    public virtual PaymentPeriod StartDeductPaymentPeriod { get; set; } = null!;

    [ForeignKey("TempEmployeeDeductionId")]
    [InverseProperty("EmployeeDeductions")]
    public virtual TempEmployeeDeduction? TempEmployeeDeduction { get; set; }
}

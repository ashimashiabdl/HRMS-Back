using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Temp_Employee_Deduction", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("DeductionTypeId", Name = "IX_Temp_Employee_Deduction_DeductionTypeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeDeductionUploadBatchId", Name = "IX_Temp_Employee_Deduction_EmployeeDeductionUploadBatchId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Temp_Employee_Deduction_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Temp_Employee_Deduction_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("StartDeductPaymentPeriodId", Name = "IX_Temp_Employee_Deduction_StartDeductPaymentPeriodId")]
public partial class TempEmployeeDeduction
{
    [Key]
    public long Id { get; set; }

    public long EmployeeDeductionUploadBatchId { get; set; }

    public long FileId { get; set; }

    public long? EmployeeId { get; set; }

    [StringLength(10)]
    public string? NationalNo { get; set; }

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

    public bool IsActive { get; set; }

    [StringLength(512)]
    public string? ParseErrorMessage { get; set; }

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

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [ForeignKey("DeductionTypeId")]
    [InverseProperty("TempEmployeeDeductions")]
    public virtual DeductionType DeductionType { get; set; } = null!;

    [ForeignKey("EmployeeId")]
    [InverseProperty("TempEmployeeDeductions")]
    public virtual Employee? Employee { get; set; }

    [ForeignKey("EmployeeDeductionUploadBatchId")]
    [InverseProperty("TempEmployeeDeductions")]
    public virtual EmployeeDeductionUploadBatch EmployeeDeductionUploadBatch { get; set; } = null!;

    [InverseProperty("TempEmployeeDeduction")]
    public virtual ICollection<EmployeeDeduction> EmployeeDeductions { get; set; } = new List<EmployeeDeduction>();

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("TempEmployeeDeductions")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("StartDeductPaymentPeriodId")]
    [InverseProperty("TempEmployeeDeductions")]
    public virtual PaymentPeriod StartDeductPaymentPeriod { get; set; } = null!;
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Employee_Deduction_Upload_Batch", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Employee_Deduction_Upload_Batch_OrganisationChartId")]
public partial class EmployeeDeductionUploadBatch
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long FileId { get; set; }

    [StringLength(256)]
    public string? UploaderUserName { get; set; }

    [StringLength(128)]
    public string? UploaderDisplayName { get; set; }

    public int TotalRowsRead { get; set; }

    public int SuccessCount { get; set; }

    public int FailedCount { get; set; }

    [StringLength(4000)]
    public string? FailedRowsJson { get; set; }

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

    [InverseProperty("EmployeeDeductionUploadBatch")]
    public virtual ICollection<EmployeeDeduction> EmployeeDeductions { get; set; } = new List<EmployeeDeduction>();

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("EmployeeDeductionUploadBatches")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [InverseProperty("EmployeeDeductionUploadBatch")]
    public virtual ICollection<TempEmployeeDeduction> TempEmployeeDeductions { get; set; } = new List<TempEmployeeDeduction>();
}

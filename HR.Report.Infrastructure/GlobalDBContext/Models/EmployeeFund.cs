using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Employee_Fund", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Employee_Fund_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("FundTypeId", Name = "IX_Employee_Fund_FundTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Employee_Fund_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("StartDeductPaymentPeriodId", Name = "IX_Employee_Fund_StartDeductPaymentPeriodId")]
public partial class EmployeeFund
{
    [Key]
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public long FundTypeId { get; set; }

    public bool IsActive { get; set; }

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

    public long StartDeductPaymentPeriodId { get; set; }

    public long OrganisationChartId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("EmployeeFunds")]
    public virtual Employee Employee { get; set; } = null!;

    [InverseProperty("EmployeeFund")]
    public virtual ICollection<FicheItem> FicheItems { get; set; } = new List<FicheItem>();

    [ForeignKey("FundTypeId")]
    [InverseProperty("EmployeeFunds")]
    public virtual FundType FundType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("EmployeeFunds")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("StartDeductPaymentPeriodId")]
    [InverseProperty("EmployeeFunds")]
    public virtual PaymentPeriod StartDeductPaymentPeriod { get; set; } = null!;
}

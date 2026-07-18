using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Temp_Personnel_Leave", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Temp_Personnel_Leave_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("LeaveTypeId", Name = "IX_Temp_Personnel_Leave_LeaveTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Temp_Personnel_Leave_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("PaymentPeriodId", Name = "IX_Temp_Personnel_Leave_PaymentPeriodId")]
[Microsoft.EntityFrameworkCore.Index("PersonnelFunctionExcelFileId", Name = "IX_Temp_Personnel_Leave_PersonnelFunctionExcelFileId")]
public partial class TempPersonnelLeave
{
    [Key]
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public long OrganisationChartId { get; set; }

    public long PaymentPeriodId { get; set; }

    public long LeaveTypeId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Minute { get; set; }

    [StringLength(150)]
    public string? Description { get; set; }

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

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Day { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Hour { get; set; }

    public long? PersonnelFunctionExcelFileId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("TempPersonnelLeaves")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("LeaveTypeId")]
    [InverseProperty("TempPersonnelLeaves")]
    public virtual LeaveType LeaveType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("TempPersonnelLeaves")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("PaymentPeriodId")]
    [InverseProperty("TempPersonnelLeaves")]
    public virtual PaymentPeriod PaymentPeriod { get; set; } = null!;

    [ForeignKey("PersonnelFunctionExcelFileId")]
    [InverseProperty("TempPersonnelLeaves")]
    public virtual PersonnelFunctionExcelFile? PersonnelFunctionExcelFile { get; set; }
}

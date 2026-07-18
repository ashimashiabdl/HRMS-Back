using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Employee_File", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Employee_File_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("FileGroupId", Name = "IX_Employee_File_FileGroupId")]
[Microsoft.EntityFrameworkCore.Index("FileId", Name = "IX_Employee_File_FileId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Employee_File_OrganisationChartId")]
public partial class EmployeeFile
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long? FileId { get; set; }

    [StringLength(500)]
    public string? Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsImage { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string Ipaddress { get; set; } = null!;

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    public long EmployeeId { get; set; }

    public long FileGroupId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [StringLength(200)]
    public string? OtherFileGroupName { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("EmployeeFiles")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("FileId")]
    [InverseProperty("EmployeeFiles")]
    public virtual File1? File { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("EmployeeFiles")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Black_List", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("BlackListEnumerationId", Name = "IX_Black_List_BlackListEnumerationId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Black_List_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Black_List_OrganisationChartId")]
public partial class BlackList
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long BlackListEnumerationId { get; set; }

    public long EmployeeId { get; set; }

    [StringLength(128)]
    public string? Comment { get; set; }

    public bool WillBeCalculated { get; set; }

    [StringLength(512)]
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

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("BlackLists")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("BlackLists")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}

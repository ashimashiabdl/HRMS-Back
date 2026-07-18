using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Personnel_ManagerList", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Personnel_ManagerList_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Personnel_ManagerList_OrganisationChartId")]
public partial class PersonnelManagerList
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    public long MaxAmount { get; set; }

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
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("PersonnelManagerLists")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("PersonnelManagerLists")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}

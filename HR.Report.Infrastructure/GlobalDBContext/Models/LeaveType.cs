using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("LeaveType", Schema = "bas")]
public partial class LeaveType
{
    [Key]
    public long Id { get; set; }

    public int? Duration { get; set; }

    public bool IsPaid { get; set; }

    [StringLength(256)]
    public string? PaymentReference { get; set; }

    [StringLength(256)]
    public string? LegalArticle { get; set; }

    [StringLength(450)]
    public string? Description { get; set; }

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

    [InverseProperty("LeaveType")]
    public virtual ICollection<EmployeeLeaveEntitlement> EmployeeLeaveEntitlements { get; set; } = new List<EmployeeLeaveEntitlement>();

    [InverseProperty("LeaveType")]
    public virtual ICollection<FicheLeaveItem> FicheLeaveItems { get; set; } = new List<FicheLeaveItem>();

    [InverseProperty("LeaveType")]
    public virtual ICollection<FunctionExcelDefinition> FunctionExcelDefinitions { get; set; } = new List<FunctionExcelDefinition>();

    [InverseProperty("LeaveType")]
    public virtual ICollection<OrganisationEmployeeTypeLeave> OrganisationEmployeeTypeLeaves { get; set; } = new List<OrganisationEmployeeTypeLeave>();

    [InverseProperty("LeaveType")]
    public virtual ICollection<OrganisationLeave> OrganisationLeaves { get; set; } = new List<OrganisationLeave>();

    [InverseProperty("LeaveType")]
    public virtual ICollection<PersonnelLeave> PersonnelLeaves { get; set; } = new List<PersonnelLeave>();

    [InverseProperty("LeaveType")]
    public virtual ICollection<TempPersonnelLeave> TempPersonnelLeaves { get; set; } = new List<TempPersonnelLeave>();
}

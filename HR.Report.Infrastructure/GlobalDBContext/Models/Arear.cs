using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Arear", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Arear_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("InterdictOrderId", Name = "IX_Arear_InterdictOrderId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Arear_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("PersonnelFunctionId", Name = "IX_Arear_PersonnelFunctionId")]
public partial class Arear
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

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

    public long? InterdictOrderId { get; set; }

    public long? PersonnelFunctionId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [InverseProperty("Arear")]
    public virtual ICollection<DeductedArear> DeductedArears { get; set; } = new List<DeductedArear>();

    [ForeignKey("EmployeeId")]
    [InverseProperty("Arears")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("InterdictOrderId")]
    [InverseProperty("Arears")]
    public virtual InterdictOrder? InterdictOrder { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("Arears")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Competency", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("CompetencyLevelId", Name = "IX_Competency_CompetencyLevelId")]
[Microsoft.EntityFrameworkCore.Index("CompetencyTypeId", Name = "IX_Competency_CompetencyTypeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Competency_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Competency_OrganisationChartId")]
public partial class Competency
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    public long? CompetencyTypeId { get; set; }

    public long? CompetencyLevelId { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public bool? Acceptable { get; set; }

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

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("Competencies")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("Competencies")]
    public virtual OrganisationChart? OrganisationChart { get; set; }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Character", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("CharacterTypeId", Name = "IX_Character_CharacterTypeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Character_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Character_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("RequiredLevelId", Name = "IX_Character_RequiredLevelId")]
public partial class Character
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    public long CharacterTypeId { get; set; }

    public long RequiredLevelId { get; set; }

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
    [InverseProperty("Characters")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("Characters")]
    public virtual OrganisationChart? OrganisationChart { get; set; }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Temp_Punishment_Encourage", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Temp_Punishment_Encourage_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Temp_Punishment_Encourage_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("TempFileId", Name = "IX_Temp_Punishment_Encourage_TempFileId")]
public partial class TempPunishmentEncourage
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public int UnitValue { get; set; }

    public long? TempFileId { get; set; }

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

    public string? NationalNo { get; set; }

    public long? EmployeeId { get; set; }

    public int? Value { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("TempPunishmentEncourages")]
    public virtual Employee? Employee { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("TempPunishmentEncourages")]
    public virtual OrganisationChart? OrganisationChart { get; set; }

    [ForeignKey("TempFileId")]
    [InverseProperty("TempPunishmentEncourages")]
    public virtual File1? TempFile { get; set; }
}

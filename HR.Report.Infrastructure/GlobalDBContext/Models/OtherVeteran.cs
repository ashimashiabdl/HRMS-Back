using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Other_Veteran", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Other_Veteran_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Other_Veteran_OrganisationChartId")]
public partial class OtherVeteran
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    public bool? IsLast { get; set; }

    public long? VeteranTypeId { get; set; }

    [StringLength(500)]
    public string? Descriptions { get; set; }

    public int? DurationYear { get; set; }

    public int? DurationMonth { get; set; }

    public int? DurationDay { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsComputeable { get; set; }

    public long? ConfirmerOrganId { get; set; }

    [StringLength(50)]
    public string? LetterNumber { get; set; }

    public int? SacrificePercent { get; set; }

    [StringLength(50)]
    public string? TrackingCode { get; set; }

    public DateOnly? LetterDate { get; set; }

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
    [InverseProperty("OtherVeterans")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OtherVeterans")]
    public virtual OrganisationChart? OrganisationChart { get; set; }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_Position_Occuption_MoreThanOne_Cach", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_Position_Occuption_MoreThanOne_Cach_OrganisationChartId")]
public partial class OrganisationPositionOccuptionMoreThanOneCach
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    [StringLength(255)]
    public string Position { get; set; } = null!;

    [StringLength(30)]
    public string? PositionCode { get; set; }

    public int? Count { get; set; }

    [StringLength(255)]
    public string? Job { get; set; }

    [StringLength(10)]
    public string? JobCode { get; set; }

    [StringLength(50)]
    public string? PersonelCode { get; set; }

    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    [StringLength(255)]
    public string? WorkPlace { get; set; }

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

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationPositionOccuptionMoreThanOneCaches")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}

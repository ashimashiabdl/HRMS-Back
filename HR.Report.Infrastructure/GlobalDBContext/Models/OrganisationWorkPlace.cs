using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_WorkPlace", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("BaseWorkPlaceId", Name = "IX_Organisation_WorkPlace_BaseWorkPlaceId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_WorkPlace_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("WorkPlaceId", Name = "IX_Organisation_WorkPlace_WorkPlaceId")]
public partial class OrganisationWorkPlace
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long WorkPlaceId { get; set; }

    public long BaseWorkPlaceId { get; set; }

    [StringLength(255)]
    public string? OrgChartWorkPlaceName { get; set; }

    [StringLength(255)]
    public string? OrgChartWorkPlaceCode { get; set; }

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

    [ForeignKey("BaseWorkPlaceId")]
    [InverseProperty("OrganisationWorkPlaces")]
    public virtual BaseWorkPlace BaseWorkPlace { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationWorkPlaceOrganisationCharts")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("WorkPlaceId")]
    [InverseProperty("OrganisationWorkPlaceWorkPlaces")]
    public virtual OrganisationChart WorkPlace { get; set; } = null!;
}

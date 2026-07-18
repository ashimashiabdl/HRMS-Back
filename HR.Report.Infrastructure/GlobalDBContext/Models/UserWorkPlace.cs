using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("User_WorkPlace", Schema = "Identity")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_User_WorkPlace_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("UserId", Name = "IX_User_WorkPlace_UserId")]
[Microsoft.EntityFrameworkCore.Index("WorkPlaceId", Name = "IX_User_WorkPlace_WorkPlaceId")]
public partial class UserWorkPlace
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    public long WorkPlaceId { get; set; }

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

    public long OrganisationChartId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("UserWorkPlaceOrganisationCharts")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserWorkPlaces")]
    public virtual AspNetUser User { get; set; } = null!;

    [ForeignKey("WorkPlaceId")]
    [InverseProperty("UserWorkPlaceWorkPlaces")]
    public virtual OrganisationChart WorkPlace { get; set; } = null!;
}

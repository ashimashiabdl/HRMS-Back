using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("User_Default_Setting", Schema = "Identity")]
[Microsoft.EntityFrameworkCore.Index("DefaultOrganId", Name = "IX_User_Default_Setting_DefaultOrganId")]
[Microsoft.EntityFrameworkCore.Index("UserId", Name = "IX_User_Default_Setting_UserId")]
public partial class UserDefaultSetting
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    public long? DefaultOrganId { get; set; }

    public long? DefaultWorkPlaceId { get; set; }

    public long? DefaultCostCenterId { get; set; }

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

    public long? DefaultOrganizationUnitId { get; set; }

    public long? DefaultPaymentPeriodId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("DefaultOrganId")]
    [InverseProperty("UserDefaultSettings")]
    public virtual OrganisationChart? DefaultOrgan { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserDefaultSettings")]
    public virtual AspNetUser User { get; set; } = null!;
}

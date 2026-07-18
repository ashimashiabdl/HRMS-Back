using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("User_PayLocation", Schema = "Identity")]
[Microsoft.EntityFrameworkCore.Index("PayLocationId", Name = "IX_User_PayLocation_PayLocationId")]
[Microsoft.EntityFrameworkCore.Index("UserId", Name = "IX_User_PayLocation_UserId")]
[Microsoft.EntityFrameworkCore.Index("UserId", "PayLocationId", Name = "IX_User_PayLocation_UserId_PayLocationId")]
public partial class UserPayLocation
{
    [Key]
    public long Id { get; set; }

    public long PayLocationId { get; set; }

    public long UserId { get; set; }

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

    [ForeignKey("PayLocationId")]
    [InverseProperty("UserPayLocations")]
    public virtual OrganisationChart PayLocation { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserPayLocations")]
    public virtual AspNetUser User { get; set; } = null!;
}

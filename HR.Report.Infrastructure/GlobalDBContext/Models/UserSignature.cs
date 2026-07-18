using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("User_Signature", Schema = "wf")]
[Microsoft.EntityFrameworkCore.Index("AspNetUsersId", Name = "IX_User_Signature_AspNetUsersId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_User_Signature_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("SignatureImageId", Name = "IX_User_Signature_SignatureImageId")]
public partial class UserSignature
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long AspNetUsersId { get; set; }

    public long SignatureImageId { get; set; }

    public bool Enabled { get; set; }

    [StringLength(512)]
    public string? SignDescription { get; set; }

    [StringLength(512)]
    public string? SignTitle { get; set; }

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
    public string? LastModifiedBy { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [InverseProperty("UserSignature")]
    public virtual ICollection<ActivityTemplate> ActivityTemplates { get; set; } = new List<ActivityTemplate>();

    [ForeignKey("AspNetUsersId")]
    [InverseProperty("UserSignatures")]
    public virtual AspNetUser AspNetUsers { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("UserSignatures")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("SignatureImageId")]
    [InverseProperty("UserSignatures")]
    public virtual File SignatureImage { get; set; } = null!;
}

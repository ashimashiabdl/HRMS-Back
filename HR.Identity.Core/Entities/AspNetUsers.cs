using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace HR.Identity.Core.Entities;

[Table("AspNetUsers", Schema = "Identity")]
public class AspNetUsers : IdentityUser<long> //HR.SharedKernel.Data.BaseEntity
{
    public long? EmployeeId { get; set; }

    [StringLength(450)]
    [IsEffectiveInGenericSearch(IsEffective = true)]

    public string FirstName { get; set; }
    [StringLength(450)]
    [IsEffectiveInGenericSearch(IsEffective = true)]

    public string NationalNo { get; set; }
    [StringLength(450)]
    [IsEffectiveInGenericSearch(IsEffective = true)]

    public string LastName { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastLoginDate { get; set; }  
    
    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastWrongAttemptDatetime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ExpiresOn { get; set; }

    // When set, after this date the user must change their password before full access
    [Column(TypeName = "datetime")]
    public DateTime? PasswordExpirationDate { get; set; }

    [StringLength(256)]
    [IsEffectiveInGenericSearch(IsEffective = true)]

    public string? Description { get; set; }

    [StringLength(45)]
    public string? AllowedIP { get; set; }

    [NotMapped]
    private new string title { get; set; }

    // When true, user must change password after login before accessing the system
    public bool MustChangePassword { get; set; }

    // Per-user password salt for custom hashing
    public byte[]? salt { get; set; }

    public long? ConfidentialityLevelId { get; set; }
    [ForeignKey("ConfidentialityLevelId")]
    public ConfidentialityLevel? ConfidentialityLevel { get; set; }

    [StringLength(512)]
    public string? DeactivationReason { get; set; }

}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("EmployeeOtp", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", "Purpose", "IsUsed", "SentAt", Name = "IX_EmployeeOtp_EmployeeId_Purpose_IsUsed_SentAt")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", "SentAt", Name = "IX_EmployeeOtp_EmployeeId_SentAt", IsDescending = new[] { false, true })]
public partial class EmployeeOtp
{
    [Key]
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    [StringLength(128)]
    public string CodeHash { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime SentAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; }

    [StringLength(45)]
    public string? CreatedByIp { get; set; }

    [StringLength(32)]
    public string Purpose { get; set; } = null!;

    [ForeignKey("EmployeeId")]
    [InverseProperty("EmployeeOtps")]
    public virtual Employee Employee { get; set; } = null!;
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Interdict_Order_Copy", Schema = "Order")]
[Microsoft.EntityFrameworkCore.Index("InterdictOrderId", Name = "IX_Interdict_Order_Copy_InterdictOrderId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Interdict_Order_Copy_OrganisationChartId")]
public partial class InterdictOrderCopy
{
    [Key]
    public long Id { get; set; }

    public long InterdictOrderId { get; set; }

    public long? OrganisationChartId { get; set; }

    public bool FinalSend { get; set; }

    [StringLength(32)]
    public string? AutomationLetterNo { get; set; }

    public DateOnly? AutomationLetterDate { get; set; }

    [StringLength(32)]
    public string? AutomationPostNo { get; set; }

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

    [ForeignKey("InterdictOrderId")]
    [InverseProperty("InterdictOrderCopies")]
    public virtual InterdictOrder InterdictOrder { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("InterdictOrderCopies")]
    public virtual OrganisationChart? OrganisationChart { get; set; }
}

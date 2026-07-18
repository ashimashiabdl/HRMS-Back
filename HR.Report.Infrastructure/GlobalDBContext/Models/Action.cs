using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Action", Schema = "wf")]
public partial class Action
{
    [Key]
    public long Id { get; set; }

    public bool? AllowComment { get; set; }

    public bool? CommentIsMandatory { get; set; }

    public bool IsDefualt { get; set; }

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
    public string? LastModifiedBy { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [InverseProperty("Action")]
    public virtual ICollection<ActivityTemplate> ActivityTemplates { get; set; } = new List<ActivityTemplate>();

    [InverseProperty("Action")]
    public virtual ICollection<Definition> Definitions { get; set; } = new List<Definition>();
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Formula_Database_Function_Definition", Schema = "For")]
[Microsoft.EntityFrameworkCore.Index("FuctionTypeId", Name = "IX_Formula_Database_Function_Definition_FuctionTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Formula_Database_Function_Definition_OrganisationChartId")]
public partial class FormulaDatabaseFunctionDefinition
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    [StringLength(256)]
    public string? EnglishName { get; set; }

    [StringLength(32)]
    public string? Schema { get; set; }

    [StringLength(255)]
    public string? FunctionName { get; set; }

    [StringLength(1024)]
    public string? Help { get; set; }

    public string? ParamsJson { get; set; }

    public string? Body { get; set; }

    public int NumberOfParameters { get; set; }

    public bool IsPublic { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string Ipaddress { get; set; } = null!;

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    public long? FuctionTypeId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("FormulaDatabaseFunctionDefinitions")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}

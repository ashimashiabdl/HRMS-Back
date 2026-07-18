using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("FundType", Schema = "bas")]
[Microsoft.EntityFrameworkCore.Index("Title", Name = "IX_FundType_title", IsUnique = true)]
public partial class FundType
{
    [Key]
    public long Id { get; set; }

    [StringLength(450)]
    public string? Description { get; set; }

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

    [InverseProperty("FundType")]
    public virtual ICollection<EmployeeFund> EmployeeFunds { get; set; } = new List<EmployeeFund>();

    [InverseProperty("FundType")]
    public virtual ICollection<OrganisationEmployeeTypeFundTypeDefinition> OrganisationEmployeeTypeFundTypeDefinitions { get; set; } = new List<OrganisationEmployeeTypeFundTypeDefinition>();

    [InverseProperty("FundType")]
    public virtual ICollection<OrganisationFundType> OrganisationFundTypes { get; set; } = new List<OrganisationFundType>();
}

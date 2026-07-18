using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Tax_DisketteWP", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Tax_DisketteWP_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("FicheId", Name = "IX_Tax_DisketteWP_FicheId")]
[Microsoft.EntityFrameworkCore.Index("TaxDisketteId", Name = "IX_Tax_DisketteWP_TaxDisketteId")]
public partial class TaxDisketteWp
{
    [Key]
    public long Id { get; set; }

    public long TaxDisketteId { get; set; }

    /// <summary>
    /// شناسه کارمند
    /// </summary>
    public long EmployeeId { get; set; }

    public int Nationality { get; set; }

    [StringLength(100)]
    public string NationalNo { get; set; } = null!;

    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [StringLength(100)]
    public string FatherName { get; set; } = null!;

    public int BirthDate { get; set; }

    public int IdentityNo { get; set; }

    [StringLength(150)]
    public string BirthPlace { get; set; } = null!;

    [StringLength(150)]
    public string EducationGrade { get; set; } = null!;

    public int InsuranceTypeId { get; set; }

    [StringLength(10)]
    public string InsuranceNo { get; set; } = null!;

    [StringLength(100)]
    public string InsuranceName { get; set; } = null!;

    [StringLength(200)]
    public string CountryOfCitizenship { get; set; } = null!;

    [StringLength(200)]
    public string CountryOfResidence { get; set; } = null!;

    [StringLength(16)]
    public string PostalCode { get; set; } = null!;

    [StringLength(1500)]
    public string Address { get; set; } = null!;

    public int Occupation { get; set; }

    [StringLength(128)]
    public string Position { get; set; } = null!;

    public int EmployeeType { get; set; }

    public int StartWorkDate { get; set; }

    public int EndWorkDate { get; set; }

    public int RetiredDate { get; set; }

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

    [StringLength(10)]
    public string ExemptionType { get; set; } = null!;

    public long FicheId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("TaxDisketteWps")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("FicheId")]
    [InverseProperty("TaxDisketteWps")]
    public virtual Fiche Fiche { get; set; } = null!;

    [ForeignKey("TaxDisketteId")]
    [InverseProperty("TaxDisketteWps")]
    public virtual TaxDiskette TaxDiskette { get; set; } = null!;
}

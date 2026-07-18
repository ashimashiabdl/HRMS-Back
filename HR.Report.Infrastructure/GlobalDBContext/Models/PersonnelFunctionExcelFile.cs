using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("PersonnelFunction_Excel_File", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("AspNetUsersId", Name = "IX_PersonnelFunction_Excel_File_AspNetUsersId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeId", Name = "IX_PersonnelFunction_Excel_File_EmployeeTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_PersonnelFunction_Excel_File_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("PaymentPeriodId", Name = "IX_PersonnelFunction_Excel_File_PaymentPeriodId")]
public partial class PersonnelFunctionExcelFile
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    [StringLength(512)]
    public string? Extension { get; set; }

    public Guid? UniqueId { get; set; }

    public long Size { get; set; }

    public byte[] Content { get; set; } = null!;

    [StringLength(512)]
    public string? MimeType { get; set; }

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

    public long AspNetUsersId { get; set; }

    public long EmployeeTypeId { get; set; }

    public long PaymentPeriodId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("AspNetUsersId")]
    [InverseProperty("PersonnelFunctionExcelFiles")]
    public virtual AspNetUser AspNetUsers { get; set; } = null!;

    [ForeignKey("EmployeeTypeId")]
    [InverseProperty("PersonnelFunctionExcelFiles")]
    public virtual EmployeeType EmployeeType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("PersonnelFunctionExcelFiles")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("PaymentPeriodId")]
    [InverseProperty("PersonnelFunctionExcelFiles")]
    public virtual PaymentPeriod PaymentPeriod { get; set; } = null!;

    [InverseProperty("PersonnelFunctionExcelFile")]
    public virtual ICollection<PersonnelLeave> PersonnelLeaves { get; set; } = new List<PersonnelLeave>();

    [InverseProperty("PersonnelFunctionExcelFile")]
    public virtual ICollection<TempPersonnelLeave> TempPersonnelLeaves { get; set; } = new List<TempPersonnelLeave>();
}

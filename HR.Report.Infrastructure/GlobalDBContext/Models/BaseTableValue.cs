using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Base_Table_Value", Schema = "bas")]
[Microsoft.EntityFrameworkCore.Index("BaseTableId", Name = "IX_Base_Table_Value_BaseTableId")]
[Microsoft.EntityFrameworkCore.Index("BaseTableId", "Title", Name = "IX_Base_Table_Value_BaseTableId_title", IsUnique = true)]
public partial class BaseTableValue
{
    [Key]
    public long Id { get; set; }

    public long BaseTableId { get; set; }

    public int Order { get; set; }

    [StringLength(256)]
    public string? Value { get; set; }

    public bool Visible { get; set; }

    [StringLength(256)]
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

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("BaseTableId")]
    [InverseProperty("BaseTableValues")]
    public virtual BaseTable BaseTable { get; set; } = null!;

    [InverseProperty("ExportType")]
    public virtual ICollection<DynamicReport> DynamicReportExportTypes { get; set; } = new List<DynamicReport>();

    [InverseProperty("FuctionType")]
    public virtual ICollection<DynamicReport> DynamicReportFuctionTypes { get; set; } = new List<DynamicReport>();

    [InverseProperty("Parameter")]
    public virtual ICollection<DynamicReportParameter> DynamicReportParameters { get; set; } = new List<DynamicReportParameter>();

    [InverseProperty("UniversityLevel")]
    public virtual ICollection<Education> EducationUniversityLevels { get; set; } = new List<Education>();

    [InverseProperty("UniversityType")]
    public virtual ICollection<Education> EducationUniversityTypes { get; set; } = new List<Education>();

    [InverseProperty("BloodGroup")]
    public virtual ICollection<Employee> EmployeeBloodGroups { get; set; } = new List<Employee>();

    [InverseProperty("Citizenship")]
    public virtual ICollection<Employee> EmployeeCitizenships { get; set; } = new List<Employee>();

    [InverseProperty("Gender")]
    public virtual ICollection<Employee> EmployeeGenders { get; set; } = new List<Employee>();

    [InverseProperty("HeadquartersOrRowType")]
    public virtual ICollection<Employee> EmployeeHeadquartersOrRowTypes { get; set; } = new List<Employee>();

    [InverseProperty("MaritalStatus")]
    public virtual ICollection<Employee> EmployeeMaritalStatuses { get; set; } = new List<Employee>();

    [InverseProperty("MartyrRelation")]
    public virtual ICollection<Employee> EmployeeMartyrRelations { get; set; } = new List<Employee>();

    [InverseProperty("Mazhab")]
    public virtual ICollection<Employee> EmployeeMazhabs { get; set; } = new List<Employee>();

    [InverseProperty("Nationality")]
    public virtual ICollection<Employee> EmployeeNationalities { get; set; } = new List<Employee>();

    [InverseProperty("Religeon")]
    public virtual ICollection<Employee> EmployeeReligeons { get; set; } = new List<Employee>();

    [InverseProperty("InsuranceDisketteStatus")]
    public virtual ICollection<InsuranceDiskette> InsuranceDisketteInsuranceDisketteStatuses { get; set; } = new List<InsuranceDiskette>();

    [InverseProperty("ReportType")]
    public virtual ICollection<InsuranceDiskette> InsuranceDisketteReportTypes { get; set; } = new List<InsuranceDiskette>();

    [InverseProperty("SupplementaryInsuranceType")]
    public virtual ICollection<Insurance> Insurances { get; set; } = new List<Insurance>();

    [InverseProperty("CheckType")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeCheck> OrganisationEmployeeTypeOrderTypeChecks { get; set; } = new List<OrganisationEmployeeTypeOrderTypeCheck>();

    [InverseProperty("PaymentType")]
    public virtual ICollection<OrganisationEmployeeTypeSettlementItem> OrganisationEmployeeTypeSettlementItems { get; set; } = new List<OrganisationEmployeeTypeSettlementItem>();

    [InverseProperty("EnterType")]
    public virtual ICollection<OrganisationFicheItem> OrganisationFicheItemEnterTypes { get; set; } = new List<OrganisationFicheItem>();

    [InverseProperty("PaymentType")]
    public virtual ICollection<OrganisationFicheItem> OrganisationFicheItemPaymentTypes { get; set; } = new List<OrganisationFicheItem>();

    [InverseProperty("ProcessArea")]
    public virtual ICollection<OrganisationJob> OrganisationJobs { get; set; } = new List<OrganisationJob>();

    [InverseProperty("MappedExcelColumn")]
    public virtual ICollection<OrganisationCoefficient> OrganisationCoefficients { get; set; } = new List<OrganisationCoefficient>();

    [InverseProperty("MappedExcelColumn")]
    public virtual ICollection<OrganisationWageItem> OrganisationWageItems { get; set; } = new List<OrganisationWageItem>();

    [InverseProperty("EnterType")]
    public virtual ICollection<PersonnelFicheItem> PersonnelFicheItems { get; set; } = new List<PersonnelFicheItem>();
}

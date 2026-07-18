using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HR.Employee.Core.Entities;

[Table("Contact_Info", Schema = "emp")]
public class ContactInfo : HR.SharedKernel.Data.BaseEntity, IEmployeeHistoryOrganisationChartId , IignoreDateRangeValidation
{
        public ContactInfo()
    {
        IPAddress = string.Empty;
        CreatedBy = string.Empty;
        LastModifiedBy = string.Empty;
        IsDeleted = false;
    }
[ForeignKey("OrganisationChart")]
    public long? OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("Employee")]
    public long EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; }public virtual BaseTableValue? AddressType { get; set; }
    public long? AddressTypeId { get; set; }
    [StringLength(300)]     
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? Address { get; set; } = string.Empty;
    [StringLength(10)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? Zipcode { get; set; } = string.Empty;
    [StringLength(32)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? Phone { get; set; } = string.Empty;
    [StringLength(32)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? EmergencyPhone { get; set; } = string.Empty;
    [StringLength(32)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? Fax { get; set; } = string.Empty;
    [StringLength(128)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? Mail { get; set; } = string.Empty;
    public long? LocationTypeId { get; set; }
    public virtual BaseTableValue? LocationType { get; set; }
    [StringLength(64)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? MobileNo { get; set; } = string.Empty;
    public bool IsVerify { get; set; } = false;
    public bool IsLast { get; set; } = false;
    [NotMapped]
    private new string title { get; set; } = string.Empty;
}

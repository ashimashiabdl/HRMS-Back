using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Contact_Info", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("AddressTypeId", Name = "IX_Contact_Info_AddressTypeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Contact_Info_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", "OrganisationChartId", "IsLast", "IsDeleted", "Id", Name = "IX_Contact_Info_EmployeeId_OrgChart_IsLast", IsDescending = new[] { false, false, false, false, true })]
[Microsoft.EntityFrameworkCore.Index("LocationTypeId", Name = "IX_Contact_Info_LocationTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Contact_Info_OrganisationChartId")]
public partial class ContactInfo
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    public long? AddressTypeId { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }

    [StringLength(10)]
    public string? Zipcode { get; set; }

    [StringLength(32)]
    public string? Phone { get; set; }

    [StringLength(32)]
    public string? Fax { get; set; }

    [StringLength(128)]
    public string? Mail { get; set; }

    public long? LocationTypeId { get; set; }

    [StringLength(64)]
    public string? MobileNo { get; set; }

    public bool IsVerify { get; set; }

    public bool IsLast { get; set; }

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

    [StringLength(32)]
    public string? EmergencyPhone { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("ContactInfos")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("ContactInfos")]
    public virtual OrganisationChart? OrganisationChart { get; set; }
}

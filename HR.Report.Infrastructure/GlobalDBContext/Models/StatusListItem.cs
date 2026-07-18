using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Status_List_Item", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("BankDisketteId", Name = "IX_Status_List_Item_BankDisketteId")]
[Microsoft.EntityFrameworkCore.Index("CurrentServiceLocationId", Name = "IX_Status_List_Item_CurrentServiceLocationId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeStatusId", Name = "IX_Status_List_Item_EmployeeStatusId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeId", Name = "IX_Status_List_Item_EmployeeTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganCodesId", Name = "IX_Status_List_Item_OrganCodesId")]
[Microsoft.EntityFrameworkCore.Index("StatusListId", Name = "IX_Status_List_Item_StatusListId")]
[Microsoft.EntityFrameworkCore.Index("WageItemId", Name = "IX_Status_List_Item_WageItemId")]
public partial class StatusListItem
{
    [Key]
    public long Id { get; set; }

    public long StatusListId { get; set; }

    public long WageItemId { get; set; }

    public long OrganCodesId { get; set; }

    public long CurrentServiceLocationId { get; set; }

    public long EmployeeStatusId { get; set; }

    public long BankDisketteId { get; set; }

    public long EmployeeTypeId { get; set; }

    public long? PersonnelCount { get; set; }

    public long Amount { get; set; }

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

    [ForeignKey("BankDisketteId")]
    [InverseProperty("StatusListItems")]
    public virtual BankDiskette BankDiskette { get; set; } = null!;

    [ForeignKey("CurrentServiceLocationId")]
    [InverseProperty("StatusListItemCurrentServiceLocations")]
    public virtual OrganisationChart CurrentServiceLocation { get; set; } = null!;

    [ForeignKey("EmployeeStatusId")]
    [InverseProperty("StatusListItems")]
    public virtual EmployeeStatus EmployeeStatus { get; set; } = null!;

    [ForeignKey("EmployeeTypeId")]
    [InverseProperty("StatusListItems")]
    public virtual EmployeeType EmployeeType { get; set; } = null!;

    [ForeignKey("OrganCodesId")]
    [InverseProperty("StatusListItemOrganCodes")]
    public virtual OrganisationChart OrganCodes { get; set; } = null!;

    [ForeignKey("StatusListId")]
    [InverseProperty("StatusListItems")]
    public virtual StatusList StatusList { get; set; } = null!;

    [ForeignKey("WageItemId")]
    [InverseProperty("StatusListItems")]
    public virtual WageItem WageItem { get; set; } = null!;
}

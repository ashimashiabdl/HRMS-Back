using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data;

[Table("Status_List_Item", Schema = "Payroll")]
public class StatusListItem : BaseEntity
{
    [ForeignKey("StatusList")]
    public long StatusListId { get; set; }
    public virtual StatusList? StatusList { get; set; }

    [ForeignKey("WageItem")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long WageItemId { get; set; }
    public virtual WageItem? WageItem { get; set; }

    [ForeignKey("OrganCodes")]
    public long OrganCodesId { get; set; }
    public virtual OrganisationChart? OrganCodes { get; set; }

    [ForeignKey("CurrentServiceLocation")]
    public long CurrentServiceLocationId { get; set; }
    public virtual OrganisationChart? CurrentServiceLocation { get; set; }

    [ForeignKey("EmployeeStatus")]
    public long EmployeeStatusId { get; set; }
    public virtual EmployeeStatus? EmployeeStatus { get; set; }

    [ForeignKey("BankDiskette")]
    public long BankDisketteId { get; set; }
    public virtual BankDiskette? BankDiskette { get; set; }

    [ForeignKey("EmployeeType")]
    public long EmployeeTypeId { get; set; }
    public virtual EmployeeType? EmployeeType { get; set; }

    public long? PersonnelCount { get; set; }

    public long Amount { get; set; }
}

using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace HR.Payroll.Core.Data;

[Table("Tax_Diskette_CostCenter", Schema = "Payroll")]
public class TaxDisketteCostCenter : BaseEntity
{
    [ForeignKey("TaxDiskette")]
    [Comment("شناسه جدول دیسکت")]
    public long TaxDisketteId { get; set; }
    public virtual TaxDiskette? TaxDiskette { get; set; }
    [ForeignKey("CostCenter")]
    public long CostCenterId { get; set; }
    public virtual OrganisationChart? CostCenter { get; set; }
    [NotMapped]
    private new string title { get; set; }
}

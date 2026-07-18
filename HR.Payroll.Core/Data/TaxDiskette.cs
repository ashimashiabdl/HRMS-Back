using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace HR.Payroll.Core.Data;

[Table("Tax_Diskette", Schema = "Payroll")]
public class TaxDiskette : SharedKernel.Data.BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("PaymentPeriod")]
    public long PaymentPeriodId { get; set; }
    public virtual PaymentPeriod? PaymentPeriod { get; set; }
    public long TaxDisketteStatusId { get; set; }

    public virtual BaseTableValue? TaxDisketteStatus { get; set; }

    [ForeignKey("BatchPayRollRequest")]
    public long? BatchPayRollRequestId { get; set; }
    public virtual BatchPayRollRequest? BatchPayRollRequest { get; set; }

    /// <summary>
    /// محاسبه دیسکت برای تمامی مراکز هزینه که در دوره جاری فیش دارند انجام شود
    /// </summary>
    [Comment("محاسبه دیسکت برای تمامی مراکز هزینه که در دوره جاری فیش دارند انجام شود")]
    public bool CalculateAllFichesInCurrentPeriod { get; set; }

    [NotMapped]
    private new string title { get; set; }
}

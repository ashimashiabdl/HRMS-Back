using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Report.Core.Entity;

[Table("PayLocation_Progress_Report", Schema = "rpt")]

public class PayLocationProgressReport : BaseEntity, IignoreDateRangeValidation
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }

    [ForeignKey("UploadedByUser")]
    public long? UploadedByUserId { get; set; }


    public string? ReportDesc { get; set; }
}

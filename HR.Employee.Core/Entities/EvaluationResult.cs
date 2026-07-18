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

namespace HR.Employee.Core.Entities;

[Table("Evaluation_Result", Schema = "emp")]
public class EvaluationResult : HR.SharedKernel.Data.BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
{
        public EvaluationResult()
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
    public virtual Employee? Employee { get; set; }
    public decimal? Average { get; set; } = 0m;
    public int Year { get; set; } = 0;
    public byte? EvaluationCoefficent { get; set; } = 0;
    public int? YearCoefficent { get; set; } = 0;
    public long? StateId { get; set; }
    public long? EvaluationGroupTypeId { get; set; }
    [NotMapped]
    private new string title { get; set; } = string.Empty;
}

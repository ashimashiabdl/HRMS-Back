using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data
{
    [Table("Arears_Status", Schema = "Payroll")]
    public class ArearsStatus : BaseEntity
    {
        //[ForeignKey("OrganisationChart")]
        //public long OrganisationChartId { get; set; }
        //public virtual OrganisationChart? OrganisationChart { get; set; }
        //[ForeignKey("Employee")]
        //public long EmployeeId { get; set; }
        //public virtual Employee.Core.Entities.Employee? Employee { get; set; }
        //[Column(TypeName = "datetime")]
        //public DateTime CalculationDate { get; set; }
        //public int MonthCount { get; set; }
        //public int CalculateMonthCount { get; set; }
        //public bool PreparedToShow { get; set; }
    }
}

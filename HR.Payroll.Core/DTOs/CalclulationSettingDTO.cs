using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class CalclulationSettingDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long? RewardFormulaId { get; set; }
        public string? RewardFormula { get; set; }
        public long? SanavatFormulaId { get; set; }
        public string? SanavatFormula { get; set; }
        public long? RewardAndSanavatStoreTypeId { get; set; }
        public string? RewardAndSanavatStoreType { get; set; }
    }
}

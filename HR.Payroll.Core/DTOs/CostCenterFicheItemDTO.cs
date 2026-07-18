using HR.SharedKernel.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.SharedKernel.Data;

namespace HR.Payroll.Core.DTOs
{
    public class CostCenterFicheItemDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long CostCenterId { get; set; }
        public string? CostCenter { get; set; }
        public long WageItemId { get; set; }
        public string? WageItem { get; set; }
        public string? Description { get; set; }
        public int? PriorityNo { get; set; }
        public bool IsFixed { get; set; }
        public bool OnceInFiche { get; set; }
        public long? Amount { get; set; }
    }
}

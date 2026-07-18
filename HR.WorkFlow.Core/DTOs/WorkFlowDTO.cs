using HR.SharedKernel.Data;
using HR.WorkFlow.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.WorkFlow.Core.DTOs
{
    public class WorkFlowDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long? WorkFlowTypeId { get; set; }
        public string? WorkFlowType { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
        public string? Description { get; set; }
    }
}

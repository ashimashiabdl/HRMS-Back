using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.DTOs
{
    public class OrganizationJobRiskAndFaultDescriptionDTO : BaseDTO
    {
        public long? OrganizationJobId { get; set; }
        public string? OrganizationJob { get; set; }
        public bool Has { get; set; }
        public long RiskOrFaultTypeId { get; set; }
        public string? RiskOrFaultType { get; set; }
    }
}

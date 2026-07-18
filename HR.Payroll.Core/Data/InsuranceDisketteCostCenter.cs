using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data
{
    [Table("Insurance_Diskette_CostCenter", Schema = "Payroll")]
    public class InsuranceDisketteCostCenter : BaseEntity
    {
        [ForeignKey("InsuranceDiskette")]
        public long InsuranceDisketteId { get; set; }
        public virtual InsuranceDiskette? InsuranceDiskette { get; set; }

        [ForeignKey("CostCenter")]
        public long CostCenterId { get; set; }
        public virtual OrganisationChart? CostCenter { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}

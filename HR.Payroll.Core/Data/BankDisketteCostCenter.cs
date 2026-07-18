using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data
{
    [Table("Bank_Diskette_CostCenter", Schema = "Payroll")]
    public class BankDisketteCostCenter : BaseEntity
    {
        [ForeignKey("BankDiskette")]
        public long BankDisketteId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual BankDiskette? BankDiskette { get; set; }
        [ForeignKey("CostCenter")]
        public long CostCenterId { get; set; }
        public virtual OrganisationChart? CostCenter { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}

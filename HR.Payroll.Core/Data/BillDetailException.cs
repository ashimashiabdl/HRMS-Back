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
    [Table("Bill_Detail_Exception", Schema = "Payroll")]
    public class BillDetailException : BaseEntity
    {
        [ForeignKey("BillDetail")]
        public long BillDetailId { get; set; }
        public virtual BillDetail? BillDetail { get; set; }

        [ForeignKey("CostCenter")]
        public long CostCenterId { get; set; }
        public virtual OrganisationChart? CostCenter { get; set; }

        public int Value { get; set; }
    }
}

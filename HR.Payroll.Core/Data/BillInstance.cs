using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data
{
    [Table("Bill_Instance", Schema = "Payroll")]
    public class BillInstance  : BaseEntity
    {
        [ForeignKey("Bill")]
        public long BillId { get; set; }
        public virtual Bill? Bill { get; set; }
        [StringLength(128)]
        public string? SerialNo { get; set; }

        [ForeignKey("PaymentPeriod")]
        public long PaymentPeriodId { get; set; }
        public virtual PaymentPeriod? PaymentPeriod { get; set; }
        [ForeignKey("SellerCostCenter")]
        public long SellerCostCenterId { get; set; }
        public virtual OrganisationChart? SellerCostCenter { get; set; }

        [ForeignKey("BuyerCostCenter")]
        public long BuyerCostCenterId { get; set; }
        public virtual OrganisationChart? BuyerCostCenter { get; set; }
        [StringLength(128)]
        public string? GeneratedBillKey { get; set; }
        public long AllAmount { get; set; }
        public long WageAmount { get; set; }
        public long TaxAmount { get; set; }
        public long PayableAmount { get; set; }
        public long? Count { get; set; }
        public long? TaxAmount3 { get; set; }
        public long? TaxAmount6 { get; set; }
        [StringLength(128)]
        public string? InvoiceNo { get; set; }
        [StringLength(128)]
        public string? ContractNo { get; set; }
    }
}

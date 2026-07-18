using HR.BaseInfo.Core.Entities;
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
    [Table("Bill", Schema = "Payroll")]
    public class Bill : BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        public virtual BaseTableValue? BillType { get; set; }
        [StringLength(128)]
        public string? BillAmountAccountNo { get; set; }
        [StringLength(128)]
        public string? BillTaxAccountNo { get; set; }
        [StringLength(128)]
        public string? BillAmountAccountInfo { get; set; }
        [StringLength(128)]
        public string? BillTaxAccountInfo { get; set; }
    }
}

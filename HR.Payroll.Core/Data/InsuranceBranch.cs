using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
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
    [Table("Insurance_Branch", Schema = "Payroll")]
    public class InsuranceBranch : SharedKernel.Data.BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]

        [ForeignKey("InsuranceType")]
        public long InsuranceTypeId { get; set; }
        public virtual InsuranceType? InsuranceType { get; set; }
        [StringLength(512)]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]

        public string? WorkshopName { get; set; }
        [StringLength(128)]
        public string? WorkshopCode { get; set; }
        [StringLength(32)]
        public string? WorkshopPhone { get; set; }
        [StringLength(512)]
        public string? WorkshopAddress { get; set; }
        public bool? IsActive { get; set; }
        /// <summary>
        /// نام کارفرما
        /// </summary>
        [StringLength(512)]
        public string? EmployerName { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}

using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.Entities
{
    [Table("Organisation_CostCenter", Schema = "Setting")]
    public class OrganisationCostCenter : HR.SharedKernel.Data.BaseEntity, IOrganisationChartId
    {
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        [ForeignKey("CostCenter")]
        public long CostCenterId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual OrganisationChart? CostCenter { get; set; }
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        /// <summary>
        /// ردیف پیمان متناظر
        /// </summary>
        [ForeignKey("PeymanRow")]
        [Comment("ردیف پیمان متناظر")]
        public long? PeymanRowId { get; set; }
        public virtual OrganisationPeymanRow? PeymanRow { get; set; }
        [StringLength(256, ErrorMessage = "طول فیلد عنوان می تواند حد اکثر 256 کاراکتر باشد")]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? OverrideCostCenterTitle { get; set; }
        /// <summary>
        /// در صد حق بالاسری مرکز هزینه
        /// </summary>
        [Comment("در صد حق بالاسری مرکز هزینه")]
        public int CostCenterPercent { get; set; }
        [Comment("کد مالی مربوط به مرکز هزینه")]
        [MaxLength(50)]
        public string? CostCenterFinancialCode { get; set; }
        [NotMapped]
        private new string title { get; set; }

    }
}

using HR.BaseInfo.Core.Entities;
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

namespace Hr.SystemSetting.Core.Entities
{
    [Table("Organisation_OrderType", Schema = "Setting")]
    public class OrganisationOrderType : HR.SharedKernel.Data.BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("OrderTypeGroup")]
        public long OrderTypeGroupId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual OrderTypeGroup? OrderTypeGroup { get; set; }
        [ForeignKey("OrderType")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrderTypeId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual OrderType? OrderType { get; set; }
        public long? OrderDirectionTypeId { get; set; }

        public virtual BaseTableValue? OrderDirectionType { get; set; }
        public bool IsBatch { get; set; }
        public bool IsPrintable { get; set; }
        public bool ShowInHistory { get; set; }
        public int? ExperienceCoefficient { get; set; }

        public int? RetiredCoefficient { get; set; }

        public int? YearCoefficient { get; set; }
        [StringLength(128)]
        public string? Code { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}

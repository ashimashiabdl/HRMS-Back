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
    [Table("Organisation_EmployeeType_OrderType_Description", Schema = "Setting")]
    public class OrganisationEmployeeTypeOrderTypeDescription : HR.SharedKernel.Data.BaseEntity, IOrganisationChartId , IignoreDateRangeValidation
    {
        [ForeignKey("OrganisationChart")]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("EmployeeType")]
      
        public long EmployeeTypeId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual EmployeeType? EmployeeType { get; set; }
        [ForeignKey("OrderType")]
   
        public long OrderTypeId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual OrderType? OrderType { get; set; }
        public bool IsDefault { get; set; }
        [StringLength(8192)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? Description { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}

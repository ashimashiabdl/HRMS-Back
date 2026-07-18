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

namespace HR.Employee.Core.Entities
{
    [Table("Appearance", Schema = "emp")]
    public class Appearance : HR.SharedKernel.Data.BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
    {
            public Appearance()
    {
        IPAddress = string.Empty;
        CreatedBy = string.Empty;
        LastModifiedBy = string.Empty;
        IsDeleted = false;
    }
[ForeignKey("OrganisationChart")]
        public long? OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("Employee")]
        public long EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }public virtual BaseTableValue? EyeColor { get; set; }public virtual BaseTableValue? SkinColor { get; set; }public virtual BaseTableValue? HairColor { get; set; }
     public long? EyeColorId { get; set; }



     public long? SkinColorId { get; set; }

     public long? HairColorId { get; set; }
     [IsEffectiveInGenericSearch(IsEffective = true)]
        [StringLength(256)]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public string? SpecificSymptoms { get; set; } = string.Empty;
        public int Weight { get; set; } = 0;
        public int Height { get; set; } = 0;
        public int FootSize { get; set; } = 0;
        [NotMapped]
        private new string title { get; set; } = string.Empty;

    }
}

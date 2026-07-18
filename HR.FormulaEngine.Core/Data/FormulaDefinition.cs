using Hr.SystemSetting.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.FormulaEngine.Core.Data
{
    [Table("Formula_Definition", Schema = "For")]
    public class FormulaDefinition : HR.SharedKernel.Data.BaseEntity, IignoreDateRangeValidation
    {
        [ForeignKey("OrganisationFormula")]
        public long Id { get; set; }
        public virtual OrganisationFormula? OrganisationFormula { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        [Required]
        public string? FormulaText { get; set; }
        public int Version { get; set; }
        [StringLength(256, ErrorMessage = "طول فیلد علت تغییر فرمول می تواند حد اکثر 256 کاراکتر باشد")]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? LastChangeReason { get; set; }
        [StringLength(256)]
        [Required]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? ErrorMessage { get; set; }

        [StringLength(2048)]
        [Required]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? Description { get; set; }
        public int SuccessRunTimeInmilliseconds { get; set; }
        [NotMapped()]
        private new string title { get; set; }
    }
}

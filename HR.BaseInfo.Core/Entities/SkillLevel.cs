using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities
{
    [Table("Skill_Level", Schema = "bas")]
    public class SkillLevel : BaseEntity
    {
        [StringLength(450)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? Description { get; set; }
    }
}



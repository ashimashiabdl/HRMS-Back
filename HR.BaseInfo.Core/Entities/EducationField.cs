using HR.SharedKernel.Attribute;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities
{
    /// <summary>
    /// رشته تحصیلی
    /// </summary>
    [Table("Education_Field", Schema = "bas")]
    public class EducationField : HR.SharedKernel.Data.BaseEntity
    {
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public int? FieldCode { get; set; }

        public int OldID { get; set; }
    }
}

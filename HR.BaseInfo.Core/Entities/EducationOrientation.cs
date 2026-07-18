using HR.SharedKernel.Attribute;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities
{
    /// <summary>
    /// گرایش های تحصیلی
    /// </summary>
    [Table("Education_Orientation", Schema = "bas")]
    public class EducationOrientation : HR.SharedKernel.Data.BaseEntity
    {
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public int? OrientationCode { get; set; }

        public int OldID { get; set; }
    }
}

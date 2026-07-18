using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities
{
    [Table("VersionChangeLog", Schema = "bas")]
    public class VersionChangeLog : BaseEntity
    {
        [Required(ErrorMessage = "شناسه نسخه الزامی می باشد")]
        public long VersionId { get; set; }

        [StringLength(20)]
        [Required(ErrorMessage = "نوع تغییر الزامی می باشد")]
        public string ChangeType { get; set; } // Added / Changed / Fixed / Removed / Security

        [StringLength(1000)]
        [Required(ErrorMessage = "توضیحات الزامی می باشد")]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string Description { get; set; }

        [ForeignKey("VersionId")]
        public virtual Version Version { get; set; }
    }
}

using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities
{
    [Table("Version", Schema = "bas")]
    public class Version : BaseEntity, IignoreDateRangeValidation
    {
        // title from BaseEntity will be used to store VersionNumber for compatibility
        [StringLength(50)]
        [Required(ErrorMessage = "شماره نسخه الزامی می باشد")]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string VersionNumber { get; set; }

        [StringLength(256)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? VersionName { get; set; }

        [Column(TypeName = "datetime")]
        [Required(ErrorMessage = "تاریخ انتشار الزامی می باشد")]
        public DateTime ReleaseDate { get; set; }

        [StringLength(20)]
        [Required(ErrorMessage = "نوع انتشار الزامی می باشد")]
        public string ReleaseType { get; set; } // Major / Minor / Patch / Hotfix

        [StringLength(20)]
        [Required(ErrorMessage = "وضعیت الزامی می باشد")]
        public string Status { get; set; } // Draft / Released / Deprecated

        [StringLength(20)]
        [Required(ErrorMessage = "محیط الزامی می باشد")]
        public string Environment { get; set; } // Production / Staging / Demo

        public bool IsBreakingChange { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<VersionChangeLog> ChangeLogs { get; set; }
    }
}

using HR.SharedKernel.Attribute;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Data
{
    public abstract class BaseEntity
    {
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long Id { get; set; }
        [StringLength(256, ErrorMessage = "طول فیلد عنوان می تواند حد اکثر 256 کاراکتر باشد")]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        [Required(ErrorMessage = "عنوان الزامی می باشد")]
        [Index(IsUnique = true)]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public string title { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastModifiedDate { get; set; }
        [StringLength(128)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? IPAddress { get; set; }
        public bool IsDeleted { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? EndDate { get; set; }
        [MaxLength(256)]
        public string? LastModifiedBy { get; set; }
        [MaxLength(256)]
        public string? CreatedBy { get; set; }
    }
}

using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.Entities
{
    [Table("Temp_Global_File", Schema = "bas")]
    public class TempGlobalFile : BaseEntity, IbaseFile
    {
        public string? Extension { get; set; }
        [NotMapped]
        public string? temp_inBase64 { get; set; }
        public Guid? UniqueId { get; set; }
        public long Size { get; set; }
        public byte[] Content { get; set; } = null!;
        [StringLength(512)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? MimeType { get; set; }
    }
}

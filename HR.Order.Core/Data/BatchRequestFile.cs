using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.Data
{
    [Table("Batch_Request_File", Schema = "Order")]
    public class BatchRequestFile : BaseEntity, IignoreDateRangeValidation, IbaseFile
    {
        [ForeignKey("BatchRequest")]
        public long BatchRequestId { get; set; }
        public virtual BatchRequest? BatchRequest { get; set; }public virtual BaseTableValue? FileType { get; set; }
        public long FileTypeId { get; set; }

        [StringLength(2048)]
        public string? MimeType { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string? Extension { get; set; }
        public Guid? UniqueId { get; set; }
        public long Size { get; set; }
        public byte[] Content { get; set; } = null!;
        [StringLength(2048)]
        public string? Description { get; set; }

    }
}

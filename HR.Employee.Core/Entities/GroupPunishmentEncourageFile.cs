using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.Entities
{
    [Table("Group_Punishment_Encourage_File", Schema = "emp")]
    public class GroupPunishmentEncourageFile : BaseEntity , IbaseFile
    {
            public GroupPunishmentEncourageFile()
    {
        IPAddress = string.Empty;
        CreatedBy = string.Empty;
        LastModifiedBy = string.Empty;
        IsDeleted = false;
    }
[ForeignKey("GroupPunishmentEncourage")]
        public long? GroupPunishmentEncourageId { get; set; }
        public virtual GroupPunishmentEncourage? GroupPunishmentEncourage { get; set; }   
        [ForeignKey("TempFile")]
        public long? TempFileId { get; set; }
        public virtual File? TempFile { get; set; }
        public string? Extension { get; set; } = string.Empty;
        public Guid? UniqueId { get; set; }
        public long Size { get; set; }
        public byte[] Content { get; set; }
        public string? MimeType { get; set; } = string.Empty;
    }
}

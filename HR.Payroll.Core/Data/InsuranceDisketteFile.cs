using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data
{
    [Table("Insurance_Diskette_File", Schema = "Payroll")]
    public class InsuranceDisketteFile : BaseEntity
    {
        [ForeignKey("InsuranceDiskette")]
        public long InsuranceDisketteId { get; set; }
        public virtual InsuranceDiskette? InsuranceDiskette { get; set; }
        // Explicit FK property to align with shadow property configuration in DbContext
        public long FileTypeId { get; set; }
        public virtual BaseTableValue? FileType { get; set; }
        public byte[]? DiskContent { get; set; } = null!;
        [NotMapped]
        public string? Content { get; set; } = null!;
        [NotMapped]
        public string? MimeType { get; set; } 
        [StringLength(64)]
        public string? Extension { get; set; }
        [StringLength(64)]
        public string? FileName { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
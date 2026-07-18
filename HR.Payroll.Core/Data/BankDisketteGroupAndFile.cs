using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HR.Payroll.Core.Data
{
    [Table("Bank_Diskette_Group_And_File", Schema = "Payroll")]
    public class BankDisketteGroupAndFile : BaseEntity
    {
        [ForeignKey("BankDiskette")]
        public long BankDisketteId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual BankDiskette? BankDiskette { get; set; }
        [ForeignKey("BankDisketteTemplate")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long BankDisketteTemplateId { get; set; }
        public virtual BankDisketteTemplate? BankDisketteTemplate { get; set; }
        public string? Content { get; set; }
        [StringLength(64)]
        public string? Extension { get; set; }
        [StringLength(64)]
        public string? FileName { get; set; }
        [NotMapped]
        private new string title { get; set; }
        [NotMapped]
        public StringBuilder? relatedStringBuilder { get; set; }
    }
}

using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.Entities
{
    /// <summary>
    /// ÃœÊ· ‘„«—Â 3 ò «»
    /// </summary>
    [Table("OrganizationJob_Task_Description", Schema = "Org")]
    public class OrganizationJobTaskDescription : BaseEntity
    {
        [ForeignKey("OrganizationJob")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganizationJobId { get; set; }
        public virtual OrganizationJob? OrganizationJob { get; set; }public virtual BaseTableValue? TaskType { get; set; }
        [MaxLength(8096)]
        public string? TaskDescription { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}

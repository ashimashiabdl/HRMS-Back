using HR.Organisation.Core.Entities;
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
    [Table("Order_Temp_File", Schema = "Order")]
    public class OrderTempFile : BaseEntity, IOrganisationChartId, IignoreDateRangeValidation, IbaseFile
    {
        [ForeignKey("OrganisationChart")]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [StringLength(2048)]
        public string? MimeType { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string? Extension { get; set; }
        public Guid? UniqueId { get; set; }
        public long Size { get; set; }
        public byte[] Content { get; set; } = null!;
    }
}

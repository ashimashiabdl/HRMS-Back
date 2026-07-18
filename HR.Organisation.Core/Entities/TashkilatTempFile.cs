using HR.SharedKernel.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.SharedKernel.Data;

namespace HR.Organisation.Core.Entities
{
    [Table("Tashkilat_Temp_File", Schema = "Org")]
    public class TashkilatTempFile : BaseEntity, IOrganisationChartId, IignoreDateRangeValidation, IbaseFile
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

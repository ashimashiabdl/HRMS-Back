using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Hr.SystemSetting.Core.Entities
{
    [Table("Organisation_MRT", Schema = "Setting")]
    public class OrganisationMRT : BaseEntity, IOrganisationChartId , IignoreDateRangeValidation , IbaseFile
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [StringLength(1024)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? Description { get; set; }
        
        public string? Extension { get; set; }
        public Guid? UniqueId { get; set; }
        public long Size { get; set; }
        public byte[] Content { get; set; } = null!;
        [StringLength(512)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? MimeType { get; set; }

    }
}
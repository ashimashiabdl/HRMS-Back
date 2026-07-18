using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HR.Organisation.Core.Entities
{
    [Table("Organisation_Position_Occuption_MoreThanOne_Cach", Schema = "Org")]
    public class OrganisationPositionOccuptionMoreThanOneCach : BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [MaxLength(255)]
        public string Position { get; set; } = null!;
        [MaxLength(30)]
        public string? PositionCode { get; set; }
        public int? Count { get; set; }
        [MaxLength(255)]
        public string? Job { get; set; }
        [MaxLength(10)]
        public string? JobCode { get; set; }
        [MaxLength(50)]
        public string? PersonelCode { get; set; }
        [MaxLength(100)]
        public string? FirstName { get; set; }
        [MaxLength(100)]
        public string? LastName { get; set; }
        [MaxLength(255)]
        public string? WorkPlace { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}

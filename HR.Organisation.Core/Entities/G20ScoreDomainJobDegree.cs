using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;



namespace HR.Organisation.Core.Entities
{
    [Table("G20_Score_Domain_JobDegree", Schema = "Org")]
    public class G20ScoreDomainJobDegree : SharedKernel.Data.BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public short JobDegree { get; set; }
        public int LowerLimit { get; set; }
        public int UpperLimit { get; set; }
        public float AmountOfRankIncrease { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}

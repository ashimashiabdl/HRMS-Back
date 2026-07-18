using HR.Identity.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.WorkFlow.Core.Data
{
    /// <summary>
    /// نقش امضا های کاربران
    /// </summary>
    [Table("User_Signature", Schema = "wf")]
    public class UserSignature : BaseEntity,IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("AspNetUsers")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long AspNetUsersId { get; set; }
        public virtual AspNetUsers? AspNetUsers { get; set; }
        [ForeignKey("SignatureImage")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long SignatureImageId { get; set; }
        public virtual HR.BaseInfo.Core.Entities.File? SignatureImage { get; set; }
        public bool Enabled { get; set; }
        [StringLength(512)]
        public string? SignDescription { get; set; } 
        [StringLength(512)]
        public string? SignTitle { get; set; }
        [NotMapped]
        public string? SignBase64Image { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}

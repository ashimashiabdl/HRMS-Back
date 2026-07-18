using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;


namespace HR.WorkFlow.Core.Data
{
    [Table("Definition", Schema = "wf")]
    public class Definition : BaseEntity
    {
        [ForeignKey("WorkFlow")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long WorkFlowId { get; set; }
        public virtual WorkFlow? WorkFlow { get; set; }
        [ForeignKey("FromNode")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long? FromNodeId { get; set; }
        public virtual Node? FromNode { get; set; }
        [ForeignKey("ToNode")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long? ToNodeId { get; set; }
        public virtual Node? ToNode { get; set; }
        [ForeignKey("Action")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long ActionId { get; set; }
        public virtual Action? Action { get; set; }
        public bool? AllowComment { get; set; }
        public bool? IsCommentRequired { get; set; }
        public bool NeedSignature { get; set; }
        public bool IsFinalTransition { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}

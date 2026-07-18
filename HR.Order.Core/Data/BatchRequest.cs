using HR.BaseInfo.Core.Entities;
using HR.Employee.Core.Entities;
using HR.Identity.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
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
    [Table("Batch_Request", Schema = "Order")]
    public class BatchRequest : BaseEntity , IignoreDateRangeValidation, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual BaseTableValue? RequestState { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual BaseTableValue? ArchiveState { get; set; }
        public long RequestStateId { get; set; }
        public long? ArchiveStateId { get; set; }
        public long RequestTypeId { get; set; }

        //[ForeignKey("RequesterEmployee")]
        //public long? RequesterEmployeeId { get; set; }
        //public virtual HR.Employee.Core.Entities.Employee? RequesterEmployee { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual BaseTableValue? RequestType { get; set; }
        [StringLength(128)]
        public string? IssuerUser { get; set; }
        [ForeignKey("AspNetUsers")]
        public long AspNetUsersId { get; set; }
        public virtual AspNetUsers? AspNetUsers { get; set; }
        [StringLength(4096)]
        public string? RequsetDescription { get; set; }
        /// <summary>
        /// ��� ͘� 
        /// </summary>
        [StringLength(8000)]
        public string? OverriddenOrderDescription { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastPoolingTime { get; set; }     
        [Column(TypeName = "datetime")]
        public DateTime? ArchiveLastPoolingTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? FinishDateTime { get; set; }  
        [Column(TypeName = "datetime")]
        public DateTime? ArchiveFinishDateTime { get; set; }
        public bool IsDone { get; set; }
        public int EmployeeCount { get; set; }
        public int SuccessCount { get; set; }
        public int PoolingEmployeeId { get; set; }
        public int ArchivePoolingEmployeeId { get; set; }
        public bool SendToCartable { get; set; }
        public bool IncludeManager { get; set; }
        public bool NeedBatchPrint { get; set; }
        public bool ForceRecruitIssue { get; set; }
        public bool IgnoreEqualToInputes { get; set; }
        public bool KeepOrderCopies { get; set; }
        public bool KeepPromissories{ get; set; }
        public bool UseMappedExcelColumns { get; set; } = true;
        public bool DatesFromWageExcel { get; set; }
        [ForeignKey("OrderType")]
        public long OrderTypeId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual OrderType? OrderType { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}

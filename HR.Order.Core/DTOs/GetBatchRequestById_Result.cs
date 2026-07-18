using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs
{
    public class GetBatchRequestById_Result
    {
        public long Id { get; set; }
        public long OrganisationChartId { get; set; }
        public long RequestStateId { get; set; }
        public string? RequestState { get; set; }   
        
        public long ArchiveStateId { get; set; }
        public string? ArchiveState { get; set; }
        public string? OverriddenOrderDescription { get; set; }
        public long RequestTypeId { get; set; }
        public string? IssuerUser { get; set; }
        
        public string? RequsetDescription { get; set; }
        public Nullable<System.DateTime> LastPoolingTime { get; set; }
        public Nullable<System.DateTime> ArchiveLastPoolingTime { get; set; }
        public bool IsDone { get; set; }
        public int EmployeeCount { get; set; }
        public int PoolingEmployeeId { get; set; }
        public int ArchivePoolingEmployeeId { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public string? IPAddress { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.DateTime> FinishDateTime { get; set; }
        public Nullable<System.DateTime> ArchiveFinishDateTime { get; set; }
        public bool IncludeManager { get; set; }
        public bool NeedBatchPrint { get; set; }
        public long OrderTypeId { get; set; }
        public string? OrderType { get; set; }
        public Nullable<long> RequesterEmployeeId { get; set; }
        public bool SendToCartable { get; set; }
        public bool IgnoreEqualToInputes { get; set; }
        public bool KeepOrderCopies { get; set; }
        public bool KeepPromissories { get; set; }
        public int SuccessCount { get; set; }
        public bool ForceRecruitIssue { get; set; }
    }
}

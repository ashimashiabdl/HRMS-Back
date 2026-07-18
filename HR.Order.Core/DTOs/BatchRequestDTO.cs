using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs
{
    public class BatchRequestDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long RequestStateId { get; set; }
        public long? WageFileTempId { get; set; }
        public long? CoefficientFileTempId { get; set; }
        public string? RequestState { get; set; }
        public string? ArchiveState { get; set; }
        //public long RequesterEmployeeId { get; set; }
        public string? RequesterEmployee { get; set; }
        public long RequestTypeId { get; set; }
        public string? RequestType { get; set; }
        [StringLength(128)]
        public string? IssuerUser { get; set; }
        public string? IssueRequsetJson { get; set; }
        [StringLength(4096)]
        public string? RequsetDescription { get; set; }
        public DateTime LastTryDate { get; set; }
        public DateTime LastPoolingTime { get; set; }
        public DateTime FinishDateTime { get; set; }
        public bool IsDone { get; set; }
        public int EmployeeCount { get; set; }
        public int SuccessCount { get; set; }
        public int PoolingEmployeeId { get; set; }

        public bool SendToCartable { get; set; }
        public bool ForceRecruitIssue { get; set; }
        public bool IncludeManager { get; set; }
        public bool NeedBatchPrint { get; set; }
        public bool IgnoreEqualToInputes { get; set; }
        public bool KeepOrderCopies { get; set; }
        public bool KeepPromissories { get; set; }
        public long OrderTypeId { get; set; }
        public string? OrderType { get; set; }

        public string? OverriddenOrderDescription { get; set; }
        public List<long>? EmployeeIdList { get; set; }
        public bool DatesFromExcel { get; set; }
        public bool UseMappedExcelColumns { get; set; } = true;
        public bool DatesFromWageExcel { get; set; }
        public long? DatesFileTempId { get; set; }

    }
}

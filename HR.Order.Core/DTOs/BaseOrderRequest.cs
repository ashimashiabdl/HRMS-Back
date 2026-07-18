using HR.SharedKernel.Excel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs
{
    public class BaseOrderRequest
    {
        public bool IsBatch { get; set; }
        public List<BatchGridModelForExcel>? wageOverRideExcel { get; set; }
        public List<BatchGridModelForExcel>? coefOverRideExcel { get; set; }
        public long EmployeeId { get; set; }
        public long PayLocationId { get; set; }
        public long EmployeeTypeId { get; set; }
        public long OrderTypeId { get; set; }
        public long? CostCenterId { get; set; }
        public long? OrganizationUnitId { get; set; }
        public long? WorkPlaceId { get; set; }
        public DateTime? StartDate { get; set; }
        public long? ProjectId { get; set; }
        public string? OrderReason { get; set; }
        public long? EmployeeStatusId { get; set; }
        public long? OrganizationJobId { get; set; }
        public long? OrganisationPositionId { get; set; }
        public bool? BuildTreeTrace { get; set; }
        public bool? DoFinalCalc { get; set; }
        public long? lastorderId { get; set; }
        public DateTime? ImpleDate
        {
            get
            {
                return StartDate == null ? DateTime.Now :  StartDate.Value.AddHours(3).AddMinutes(30);
            }
        }
        public DateTime? EndDate { get; set; }
        public long IssueTypeId { get; set; }
        public long? CorrectionOrderId { get; set; }
        public List<coeficentItem>? CoeficentItems { get; set; }
        public List<WageItem>? WageItems { get; set; }
        public InterdictOrderFlatDTO? InterdictOrderDTO { set; get; }

    }
}

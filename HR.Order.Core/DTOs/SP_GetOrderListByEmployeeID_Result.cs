using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs
{
    public class SP_GetOrderListByEmployeeID_Result
    {
        public long Id { get; set; }
        public long RecruitOrderId { get; set; }
        public string? Status { get; set; }
        public long StatusId { get; set; }
        public string? OrderName { get; set; }
        public string? CostCenterDesc { get; set; }
        public Nullable<long> OrganisationPositionId { get; set; }
        public string? PostDesc { get; set; }
        public string? JobDesc { get; set; }
        public Nullable<int> JobDegree { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> ExpireDate { get; set; }
        public Nullable<short> OrderSerial { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public long IssueType { get; set; }
        public string? title { get; set; }
        public Nullable<decimal> SumWageFactors { get; set; }
        public Nullable<bool> IsPrintable { get; set; }
        public long PaylocationID { get; set; }
        public string? PaylocationName { get; set; }
        public Nullable<short> CorrigendumInterdictSerial { get; set; }
        public Nullable<long> CorrigendumRecInterdictID { get; set; }
        public Nullable<long> LastInterdictOrderId { get; set; }
        public long OrderTypeId { get; set; }
        public long EmployeeTypeId { get; set; }
        public string? EmployeeType { get; set; }
        public Nullable<long> EmployeeStatusId { get; set; }
        public string? EmployeeStatus { get; set; }
        public Nullable<long> OrganisationUnitId { get; set; }
        public string? OrganisationUnit { get; set; }
        public Nullable<long> WorkPlaceId { get; set; }
        public string? WorkPlaceName { get; set; }
        public string? IssuerName { get; set; }
        public Nullable<long> RecRank { get; set; }
        public Nullable<int> TotalCount { get; set; }
        public bool IsArrears { get; set; }
        public Nullable<System.DateTime> PayRollRealExecuteDate { get; set; }

        public Nullable<long> ArearsStatusId { get; set; }

        public string? ArearsStatus { get; set; }
    }
}

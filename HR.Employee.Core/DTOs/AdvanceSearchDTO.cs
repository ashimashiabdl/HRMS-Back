using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs
{
    public class AdvanceSearchDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PersonelCode { get; set; }
        public string? IdentityNo { get; set; }
        public string? NationalNo { get; set; }
        public string? ActiveName { get; set; }
        public long? CostCenterId { get; set; }
        public int limit { get; set; }
        public long? OrganizationUnitId { get; set; }
        public long? WorkPlaceId { get; set; }
        public long? EmployeeStatusId { get; set; }
        public long? EmployeeTypeId { get; set; }
        public long? PayLocationId { get; set; }
        public long? EducationGradeId { get; set; }
        public long? JobNatureId { get; set; }
        public long? OrganizationJobId { get; set; }
        public long? GenderId { get; set; }
        public long? MaritalStatusId { get; set; }

        public bool IsRecruitment { get; set; }
        public bool IsQuickSearch { get; set; }
        public string? SortBy { get; set; } = "FirstName";
        public string? SortDirection { get; set; } = "ASC";
    }
}

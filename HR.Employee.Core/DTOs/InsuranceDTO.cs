using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs
{
    public class InsuranceDTO : BaseDTO
    {
        public long? OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long EmployeeId { get; set; }
        public string? Employee { get; set; }
        public long? InsWorkShopTypeId { get; set; }
        public string? InsWorkShopType { get; set; }
        public long? InsuranceBranchId { get; set; }
        public string? InsuranceBranch { get; set; }
        public long? InsuranceTypeId { get; set; }
        public string? InsuranceType { get; set; }
        public int? AccDay { get; set; }
        public bool? IsLast { get; set; }
        public bool? IsComputable { get; set; }
        [StringLength(32)]
        public string InsuranceNumber { get; set; } = null!;
        public bool? HasSupplementaryInsurance { get; set; }
        public long? SupplementaryInsuranceTypeId { get; set; }
        public string? SupplementaryInsuranceType { get; set; }
        public long? InsuranceSubmissionCityId { get; set; }
        public string? InsuranceSubmissionCity { get; set; }
    }
}

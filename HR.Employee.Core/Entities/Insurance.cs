using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Employee.Core.Entities
{
    [Table("Insurance", Schema = "emp")]
    public class Insurance : BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
    {
        public Insurance()
        {
            IPAddress = string.Empty;
            CreatedBy = string.Empty;
            LastModifiedBy = string.Empty;
            IsDeleted = false;
        }
        [ForeignKey("OrganisationChart")]
        public long? OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("Employee")]
        public long EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }
        public long? InsWorkShopTypeId { get; set; }
        public virtual BaseTableValue? InsWorkShopType { get; set; }
        //[ForeignKey("InsuranceBranch")]
        public long? InsuranceBranchId { get; set; }// Fk to payroll table
        //public virtual BaseTableValue? InsuranceBranch { get; set; }
        //[ForeignKey("InsuranceType")]
        public long? InsuranceTypeId { get; set; }
        //public virtual BaseTableValue? InsuranceType { get; set; }
        public int? AccDay { get; set; } = 0;
        public bool? IsLast { get; set; } = false;
        public bool? IsComputable { get; set; } = false;
        [StringLength(32)]
        public string InsuranceNumber { get; set; } = string.Empty;
        // New fields
        public bool? HasSupplementaryInsurance { get; set; } = false;
        // Supplementary insurance type (BaseTableId = 79)
        public long? SupplementaryInsuranceTypeId { get; set; }
        public virtual BaseTableValue? SupplementaryInsuranceType { get; set; }
        [ForeignKey("InsuranceSubmissionCity")]
        public long? InsuranceSubmissionCityId { get; set; }
        public virtual HR.BaseInfo.Core.Entities.Places? InsuranceSubmissionCity { get; set; }
        [NotMapped]
        private new string title { get; set; } = string.Empty;
    }
}

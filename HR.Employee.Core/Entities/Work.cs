using HR.BaseInfo.Core.Entities;
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

namespace HR.Employee.Core.Entities
{
    [Table("Work", Schema = "emp")]
    public class Work : BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
    {
            public Work()
    {
        IPAddress = string.Empty;
        CreatedBy = string.Empty;
        LastModifiedBy = string.Empty;
        IsDeleted = false;
    }
[ForeignKey("OrganisationChart")]
        public long? OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; } 
        
        
        
        [ForeignKey("OrgChartWorkPlace")]
        public long? OrgChartWorkPlaceId { get; set; }
        public virtual OrganisationChart? OrgChartWorkPlace { get; set; }



        [ForeignKey("EmployeeType")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long EmployeeTypeId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual EmployeeType? EmployeeType { get; set; }

        [ForeignKey("EmployeeStatus")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long EmployeeStatusId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual EmployeeStatus? EmployeeStatus { get; set; }

        [ForeignKey("Employee")]
        public long EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }
        public long? IndustryTypeId { get; set; }
        public long? ActivityTypeId { get; set; }
        public long? InsuranceTypeId { get; set; }
        public long? RelatedTypeId { get; set; }
        public long? LeaveDueToWorkTypeId { get; set; }


        [ForeignKey("EducationGrade")]
        public long? EducationGradeId { get; set; }
            public virtual EducationGrade? EducationGrade { get; set; }
        public long? StatusId { get; set; }

        [StringLength(256)]
        public string? WorkPlaceDesc { get; set; } = string.Empty;

        [StringLength(128)]
        public string? LetterNumber { get; set; } = string.Empty;
        [StringLength(256)]
        public string? LastTitle { get; set; } = string.Empty;
        [Column(TypeName = "date")]
        public DateTime? WorkingFrom { get; set; }
        [Column(TypeName = "date")]
        public DateTime? LetterDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? WorkingTo { get; set; }
        [Column(TypeName = "date")]
        public DateTime? HistoryDate { get; set; }
        public byte InsHsyYear { get; set; } = 0;
        public byte InsHsyMonth { get; set; } = 0;
        public byte InsHsyDay { get; set; } = 0;
        public byte AcptInsHsyYear { get; set; } = 0;
        public byte AcptInsHsyMonth { get; set; } = 0;
        public byte AcptInsHsyDay { get; set; } = 0;
        public long? LastSalary { get; set; }
        [StringLength(500)]
        public string? Description { get; set; } = string.Empty;
    
        public bool? IsComputeable { get; set; } = false;

        public int? ExperienceMult { get; set; } = 0;

        public int? RetiredMult { get; set; } = 0;

        public int? YearMult { get; set; } = 0;
    }
}

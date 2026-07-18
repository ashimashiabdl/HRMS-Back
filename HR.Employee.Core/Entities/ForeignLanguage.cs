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

namespace HR.Employee.Core.Entities;

[Table("Foreign_Language", Schema = "emp")]
public class ForeignLanguage : HR.SharedKernel.Data.BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
{
        public ForeignLanguage()
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
    [Column(TypeName = "date")]
    public DateTime Initdate { get; set; }
    [Column(TypeName = "date")]
    public DateTime? Expiredate { get; set; }
    public long? LanguageId { get; set; }
    public long? LanguageskillId { get; set; }
    public long? LevelId { get; set; }


    [StringLength(50)]
    public string? Languagescore { get; set; } = string.Empty;
    [StringLength(64)]
    public string? OtherLanguageName { get; set; } = string.Empty;
    public long? ApprovedbyId { get; set; }
    public bool? Acceptable { get; set; } = false;

    [StringLength(500, ErrorMessage = "ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ اکï؟½ï؟½ 500 ï؟½ï؟½ï؟½اکï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½")]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? Description { get; set; } = string.Empty;
    [NotMapped]
    private new string title { get; set; } = string.Empty;
}

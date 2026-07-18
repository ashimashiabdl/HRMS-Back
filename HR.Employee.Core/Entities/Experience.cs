using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.BaseInfo.Core.Entities;

namespace HR.Employee.Core.Entities;

[Table("Experience", Schema = "emp")]
public class Experience : BaseEntity, IignoreDateRangeValidation
{
        public Experience()
    {
        IPAddress = string.Empty;
        CreatedBy = string.Empty;
        LastModifiedBy = string.Empty;
        IsDeleted = false;
    }
[ForeignKey("OrganisationChart")]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }

    [ForeignKey("Employee")]
    public long EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; }

    [ForeignKey("HistoryType")]
    public long? HistoryTypeId { get; set; }
    public virtual HistoryType? HistoryType { get; set; }

    // ظ…ط¯طھ
    [StringLength(6)]
    public string? Duration { get; set; } = string.Empty;

    // ظ†ظˆط¹ ط³ط§ط¨ظ‚ظ‡: ط¯ط§ط®ظ„ ط³ط§ط²ظ…ط§ظ† (true) / ط®ط§ط±ط¬ ط³ط§ط²ظ…ط§ظ† (false)
    public bool IsInternal { get; set; } = false;

    // ظ‚ط§ط¨ظ„ ظ‚ط¨ظˆظ„ ط¨ظˆط¯ظ†
    public bool IsAcceptable { get; set; } = false;

    // ط¯ط±طµط¯ ظ‚ط§ط¨ظ„ ظ‚ط¨ظˆظ„ ط¨ظˆط¯ظ† (ط¯ط± طµظˆط±طھ ظ‚ط§ط¨ظ„ ظ‚ط¨ظˆظ„ ط¨ظˆط¯ظ†)
    public int? AcceptablePercent { get; set; } = 0;

    // ط¹ظ†ظˆط§ظ† ط´ط±ع©طھ/ط³ط§ط²ظ…ط§ظ†
    [StringLength(250)]
    public string? CompanyTitle { get; set; } = string.Empty;

    [NotMapped]
    private new string title { get; set; } = string.Empty;
}

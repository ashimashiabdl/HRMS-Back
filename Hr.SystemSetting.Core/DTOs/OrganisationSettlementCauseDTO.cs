using HR.SharedKernel.Data;

namespace Hr.SystemSetting.Core.DTOs;

public class OrganisationSettlementCauseDTO : BaseDTO
{
    public long SettlementCauseId { get; set; }
    public string? SettlementCauseTitle { get; set; }
}

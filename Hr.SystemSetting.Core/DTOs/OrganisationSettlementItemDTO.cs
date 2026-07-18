using HR.SharedKernel.Data;

namespace Hr.SystemSetting.Core.DTOs;

public class OrganisationSettlementItemDTO : BaseDTO
{
    public long SettlementItemId { get; set; }
    public string? SettlementItemTitle { get; set; }
}

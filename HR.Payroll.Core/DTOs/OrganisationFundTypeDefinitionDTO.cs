using HR.SharedKernel.Data;

namespace HR.Payroll.Core.DTOs;

public class OrganisationFundTypeDefinitionDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }

    public long FundTypeId { get; set; }
    public string? FundType { get; set; }
}


namespace HR.Payroll.Core.DTOs;

public class EmployeeSettlementFicheValidationDTO
{
    public int FicheCount { get; set; }

    public bool HasFicheInRange { get; set; }

    public bool RequiresConfirmation { get; set; }

    public string? StartDateShamsi { get; set; }

    public string? EndDateShamsi { get; set; }

    public string? Message { get; set; }
}

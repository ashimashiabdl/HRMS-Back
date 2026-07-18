namespace HR.Order.Core.DTOs;

public class UpdateDraftOrderDatesDTO
{
    public long Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

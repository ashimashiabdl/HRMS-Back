using HR.SharedKernel.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs;

public class IssueOrderRequest : BaseOrderRequest
{
    public long StatusId { get; set; }
    public string? Description { get; set; }
    public DateTime? ImpleDate
    {
        get
        {
            return StartDate == null ? DateTime.Now : StartDate.Value.AddHours(3).AddMinutes(30);
        }
    }
    public long UserId { get; set; }
    public bool IsOutDate { get; set; }
  
    public bool IgnoreEqualToInputesInBatch { get; set; }
    public long? OrderLevelId { get; set; }

    public List<long>? OrderCopyList { get; set; }
    public List<InterdictOrderPromissoryDTO>? InterdictOrderPromissories { get; set; }


}


public class coeficentItem
{

    public long CoefficientId { get; set; }
    public long EnterTypeId { get; set; }
    public double Value { get; set; }
}

public class WageItem
{
    public long WageItemId { get; set; }
    public long EnterTypeId { get; set; }
    public int Value { get; set; }
}

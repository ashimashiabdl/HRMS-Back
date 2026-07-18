using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

/// <summary>
/// وضعیت تسویه حساب
/// </summary>
[Table("Settlement_Status", Schema = "bas")]
public class SettlementStatus : HR.SharedKernel.Data.BaseEntity
{
    public int StatusCode { get; set; }
}

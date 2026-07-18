using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

[Table("AttendanceHoliday", Schema = "bas")]
public class AttendanceHoliday : BaseEntity
{
    [Column(TypeName = "datetime")]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public DateTime HolidayDate { get; set; }

    public bool IsOfficial { get; set; }

    [ForeignKey("Place")]
    public long PlaceId { get; set; }
    public virtual Places? Place { get; set; }
}

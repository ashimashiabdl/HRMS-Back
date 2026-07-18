using HR.SharedKernel.Data;

namespace HR.BaseInfo.Core.DTOs;

public class AttendanceHolidayDTO : BaseDTO
{
    public DateTime HolidayDate { get; set; }
    public bool IsOfficial { get; set; }
    public long PlaceId { get; set; }
    public string? Place { get; set; }
}

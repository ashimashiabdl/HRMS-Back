using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("AttendanceHoliday", Schema = "bas")]
[Microsoft.EntityFrameworkCore.Index("PlaceId", Name = "IX_AttendanceHoliday_PlaceId")]
public partial class AttendanceHoliday
{
    [Key]
    public long Id { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime HolidayDate { get; set; }

    public bool IsOfficial { get; set; }

    public long PlaceId { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string? Ipaddress { get; set; }

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [InverseProperty("Holiday")]
    public virtual ICollection<AttendanceCalendar> AttendanceCalendars { get; set; } = new List<AttendanceCalendar>();

    [ForeignKey("PlaceId")]
    [InverseProperty("AttendanceHolidays")]
    public virtual Place Place { get; set; } = null!;
}

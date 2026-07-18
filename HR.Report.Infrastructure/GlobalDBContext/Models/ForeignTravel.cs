using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Foreign_Travel", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Foreign_Travel_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Foreign_Travel_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("PlaceId", Name = "IX_Foreign_Travel_PlaceId")]
public partial class ForeignTravel
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    [StringLength(500)]
    public string? Descriptions { get; set; }

    public DateOnly? LetterDate { get; set; }

    [StringLength(50)]
    public string? LetterNumber { get; set; }

    public long? PlaceId { get; set; }

    [StringLength(256)]
    public string? CountryNames { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public long? StatusId { get; set; }

    public long? TravelTypeId { get; set; }

    [StringLength(1024)]
    public string? MissionSubject { get; set; }

    public int? MissionCost { get; set; }

    public long? MissionTypeId { get; set; }

    public long? ReasonId { get; set; }

    public int? CountryCount { get; set; }

    public int? ArchiveId { get; set; }

    [StringLength(1024)]
    public string? CountryList { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string Ipaddress { get; set; } = null!;

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("ForeignTravels")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("ForeignTravels")]
    public virtual OrganisationChart? OrganisationChart { get; set; }

    [ForeignKey("PlaceId")]
    [InverseProperty("ForeignTravels")]
    public virtual Place? Place { get; set; }
}

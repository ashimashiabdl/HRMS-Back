using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Places", Schema = "bas")]
[Microsoft.EntityFrameworkCore.Index("ParentPlaceId", Name = "IX_Places_ParentPlaceId")]
[Microsoft.EntityFrameworkCore.Index("PlaceTypeId", Name = "IX_Places_PlaceTypeId")]
public partial class Place
{
    [Key]
    public long Id { get; set; }

    public long? ParentPlaceId { get; set; }

    public long? PlaceTypeId { get; set; }

    [StringLength(450)]
    public string? Description { get; set; }

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

    [StringLength(3)]
    public string? TaxCode { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [InverseProperty("Place")]
    public virtual ICollection<AttendanceHoliday> AttendanceHolidays { get; set; } = new List<AttendanceHoliday>();

    [InverseProperty("EducationPlaces")]
    public virtual ICollection<Education> Educations { get; set; } = new List<Education>();

    [InverseProperty("BirthPlace")]
    public virtual ICollection<Employee> EmployeeBirthPlaces { get; set; } = new List<Employee>();

    [InverseProperty("IssuePlace")]
    public virtual ICollection<Employee> EmployeeIssuePlaces { get; set; } = new List<Employee>();

    [InverseProperty("ServicePlace")]
    public virtual ICollection<Employee> EmployeeServicePlaces { get; set; } = new List<Employee>();

    [InverseProperty("BirthPlace")]
    public virtual ICollection<Family> Families { get; set; } = new List<Family>();

    [InverseProperty("Place")]
    public virtual ICollection<ForeignTravel> ForeignTravels { get; set; } = new List<ForeignTravel>();

    [InverseProperty("InsuranceSubmissionCity")]
    public virtual ICollection<Insurance> Insurances { get; set; } = new List<Insurance>();

    [InverseProperty("BirthPlace")]
    public virtual ICollection<InterdictOrder> InterdictOrderBirthPlaces { get; set; } = new List<InterdictOrder>();

    [InverseProperty("IssuePlace")]
    public virtual ICollection<InterdictOrder> InterdictOrderIssuePlaces { get; set; } = new List<InterdictOrder>();

    [InverseProperty("ParentPlace")]
    public virtual ICollection<Place> InverseParentPlace { get; set; } = new List<Place>();

    [ForeignKey("ParentPlaceId")]
    [InverseProperty("InverseParentPlace")]
    public virtual Place? ParentPlace { get; set; }
}

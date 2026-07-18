using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Identity.Core.Entities;

[Table("AspNetRoles", Schema = "Identity")]
public class AspNetRoles : IdentityRole<long>
{
    [StringLength(256, ErrorMessage = "طول فیلد عنوان می تواند حد اکثر 256 کاراکتر باشد")]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [Required(ErrorMessage = "عنوان الزامی می باشد")]
    [Index(IsUnique = true)]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public string? PersianName { get; set; }

    //[ForeignKey("OrganisationChart")]
    //[IsEffectiveInDateOverLapChecking(IsEffective = true)]
    //public long OrganisationChartId { get; set; }
    //public virtual OrganisationChart? OrganisationChart { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }
}

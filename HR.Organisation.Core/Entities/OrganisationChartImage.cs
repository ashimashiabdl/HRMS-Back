using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Organisation.Core.Entities;

[Table("Organisation_Chart_Image", Schema = "Org")]
public class OrganisationChartImage : HR.SharedKernel.Data.BaseEntity, IOrganisationChartId , IbaseFile
{
    [ForeignKey("OrganisationChart")]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [StringLength(2048)]
    public string? MimeType { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public string? Extension { get; set; }
    public Guid? UniqueId { get; set; }
    public long Size { get; set; }
    public byte[] Content { get; set; } = null!;
}

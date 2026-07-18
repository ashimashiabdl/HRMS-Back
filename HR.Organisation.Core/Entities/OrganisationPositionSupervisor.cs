using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Organisation.Core.Entities;

[Table("Organisation_Position_Supervisor", Schema = "Org")]
public class OrganisationPositionSupervisor : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }

    public long EmployeeID { get; set; }

    [ForeignKey("OrganisationPosition")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationPositionId { get; set; }
    public virtual OrganisationPosition? OrganisationPosition { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    public bool IsMain { get; set; }

    [NotMapped]
    private new string title { get; set; }
}

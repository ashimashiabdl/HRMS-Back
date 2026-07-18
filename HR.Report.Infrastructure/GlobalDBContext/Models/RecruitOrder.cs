using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Recruit_Order", Schema = "Order")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_RecruitOrder_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeStatusId", "EmployeeTypeId", Name = "IX_RecruitOrder_EmploymentType")]
[Microsoft.EntityFrameworkCore.Index("PayLocationId", "CostCenterId", "OrganizationUnitId", "WorkPlaceId", Name = "IX_RecruitOrder_OrgStructure")]
[Microsoft.EntityFrameworkCore.Index("OrganizationJobId", "OrganisationPositionId", "ProjectId", Name = "IX_RecruitOrder_PositionProject")]
[Microsoft.EntityFrameworkCore.Index("CostCenterId", Name = "IX_Recruit_Order_CostCenterId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Recruit_Order_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", "IsDeleted", "Id", Name = "IX_Recruit_Order_EmployeeId_IsDeleted_Id")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Recruit_Order_EmployeeId_Lookup")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", "PayLocationId", "IsDeleted", Name = "IX_Recruit_Order_EmployeeId_PayLocationId_IsDeleted")]
[Microsoft.EntityFrameworkCore.Index("EmployeeStatusId", Name = "IX_Recruit_Order_EmployeeStatusId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeId", Name = "IX_Recruit_Order_EmployeeTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationPositionId", Name = "IX_Recruit_Order_OrganisationPositionId")]
[Microsoft.EntityFrameworkCore.Index("OrganizationJobId", Name = "IX_Recruit_Order_OrganizationJobId")]
[Microsoft.EntityFrameworkCore.Index("OrganizationUnitId", Name = "IX_Recruit_Order_OrganizationUnitId")]
[Microsoft.EntityFrameworkCore.Index("PayLocationId", Name = "IX_Recruit_Order_PayLocationId")]
[Microsoft.EntityFrameworkCore.Index("PayLocationId", "EmployeeId", Name = "IX_Recruit_Order_PayLocationId_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("PayLocationId", "Id", Name = "IX_Recruit_Order_PayLocationId_Id")]
[Microsoft.EntityFrameworkCore.Index("ProjectId", Name = "IX_Recruit_Order_ProjectId")]
[Microsoft.EntityFrameworkCore.Index("WorkPlaceId", Name = "IX_Recruit_Order_WorkPlaceId")]
public partial class RecruitOrder
{
    [Key]
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public long PayLocationId { get; set; }

    public long CostCenterId { get; set; }

    public long? OrganizationUnitId { get; set; }

    public long? WorkPlaceId { get; set; }

    public long EmployeeStatusId { get; set; }

    public long EmployeeTypeId { get; set; }

    public long? OrganizationJobId { get; set; }

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

    public long? OrganisationPositionId { get; set; }

    public long? ProjectId { get; set; }

    public int? CostCenterPercent { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("CostCenterId")]
    [InverseProperty("RecruitOrderCostCenters")]
    public virtual OrganisationChart CostCenter { get; set; } = null!;

    [ForeignKey("EmployeeId")]
    [InverseProperty("RecruitOrders")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("EmployeeStatusId")]
    [InverseProperty("RecruitOrders")]
    public virtual EmployeeStatus EmployeeStatus { get; set; } = null!;

    [ForeignKey("EmployeeTypeId")]
    [InverseProperty("RecruitOrders")]
    public virtual EmployeeType EmployeeType { get; set; } = null!;

    [InverseProperty("RecruitOrder")]
    public virtual ICollection<InterdictOrder> InterdictOrders { get; set; } = new List<InterdictOrder>();

    [ForeignKey("OrganisationPositionId")]
    [InverseProperty("RecruitOrders")]
    public virtual OrganisationPosition? OrganisationPosition { get; set; }

    [ForeignKey("OrganizationJobId")]
    [InverseProperty("RecruitOrders")]
    public virtual OrganisationJob? OrganizationJob { get; set; }

    [ForeignKey("OrganizationUnitId")]
    [InverseProperty("RecruitOrderOrganizationUnits")]
    public virtual OrganisationChart? OrganizationUnit { get; set; }

    [ForeignKey("PayLocationId")]
    [InverseProperty("RecruitOrderPayLocations")]
    public virtual OrganisationChart PayLocation { get; set; } = null!;

    [ForeignKey("ProjectId")]
    [InverseProperty("RecruitOrders")]
    public virtual Project? Project { get; set; }

    [ForeignKey("WorkPlaceId")]
    [InverseProperty("RecruitOrderWorkPlaces")]
    public virtual OrganisationChart? WorkPlace { get; set; }
}

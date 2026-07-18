using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.Entities
{
    [Table("Organisation_EmployeeType_OrderType_CanChange", Schema = "Setting")]
    public class OrganisationEmployeeTypeOrderTypeCanChange : HR.SharedKernel.Data.BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("EmployeeType")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long EmployeeTypeId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual EmployeeType? EmployeeType { get; set; }
        [ForeignKey("OrderType")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrderTypeId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual OrderType? OrderType { get; set; }
        public bool CanImplDate { get; set; }
        public bool CanExpireDate { get; set; }
        public bool CanPayLocationId { get; set; }
        public bool CanPositionId { get; set; }
        public bool CanJobID { get; set; }
        public bool CanOrganizationUnitId { get; set; }
        public bool CanWorkPlaceId { get; set; }
        public bool CanEmployeeStatusId { get; set; }
        public bool CanEmployeeTypeId { get; set; }
        public bool CanCostCenterId { get; set; }
        public bool CanProjectId { get; set; }
        [ForeignKey("DefaultEmpType")]
        public Nullable<long> DefaultEmpTypeId { get; set; }
        public virtual EmployeeType? DefaultEmpType { get; set; }
        [ForeignKey("DefaultEmpStatus")]
        public Nullable<long> DefaultEmpStatusId { get; set; }
        public virtual EmployeeStatus? DefaultEmpStatus { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}

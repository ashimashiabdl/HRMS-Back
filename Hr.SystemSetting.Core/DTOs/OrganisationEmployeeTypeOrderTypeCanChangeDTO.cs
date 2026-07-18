using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.DTOs
{
    public class OrganisationEmployeeTypeOrderTypeCanChangeDTO : BaseDTO
    {
        public long EmployeeTypeId { get; set; }
        public string? EmployeeTypeTitle { get; set; }
        public long OrderTypeId { get; set; }
        public string? OrderTypeTitle { get; set; }
        public bool? CanImplDate { get; set; }
        public bool? CanExpireDate { get; set; }
        public bool? CanPayLocationId { get; set; }
        public bool? CanPositionId { get; set; }
        public bool? CanJobID { get; set; }
        public bool? CanOrganizationUnitId { get; set; }
        public bool? CanWorkPlaceId { get; set; }
        public bool? CanEmployeeStatusId { get; set; }
        public bool? CanEmployeeTypeId { get; set; }
        public bool? CanCostCenterId { get; set; }
        public bool? CanProjectId { get; set; }
        public Nullable<long> DefaultEmpTypeId { get; set; }
        public string? DefaultEmpTypeTitle { get; set; }
        public Nullable<long> DefaultEmpStatusId { get; set; }
        public string? DefaultEmpStatusTitle { get; set; }
    }
}

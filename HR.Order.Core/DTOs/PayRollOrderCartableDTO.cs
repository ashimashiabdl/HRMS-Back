using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs
{
    public class PayRollOrderCartableDTO
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PersonelCode { get; set; }
        public string? NationalNo { get; set; }
        public long PayLocationId { get; set; }
        public string? PayLocation { get; set; }
        public long CostCenterId { get; set; }
        public string? CostCenter { get; set; }
        public long? OrganizationUnitId { get; set; }
        public string? OrganizationUnit { get; set; }
        public long? WorkPlaceId { get; set; }
        public string? WorkPlace { get; set; }
        public long? ProjectId { get; set; }
        public string? Project { get; set; }
        public long EmployeeStatusId { get; set; }
        public string? EmployeeStatus { get; set; }
        public long EmployeeTypeId { get; set; }
        public string? EmployeeType { get; set; }
        public long? OrganizationJobId { get; set; }
        public string? OrganizationJob { get; set; }
        public long? OrganisationPositionId { get; set; }
        public string? OrganisationPosition { get; set; }
        public long StatusId { get; set; }
        public string? Status { get; set; }
        public short? Serial { get; set; }
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? EndDate { get; set; }

        public DateTime? CreateDate { get; set; }
        public long OrderTypeId { get; set; }
        public string? OrderType { get; set; }

        public bool Selected { get; set; }
    }
}

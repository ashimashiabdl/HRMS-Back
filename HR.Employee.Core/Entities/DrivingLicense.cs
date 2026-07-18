using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.Entities
{
    [Table("Driving_License", Schema = "emp")]
    public class DrivingLicense : BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
    {
            public DrivingLicense()
    {
        IPAddress = string.Empty;
        CreatedBy = string.Empty;
        LastModifiedBy = string.Empty;
        IsDeleted = false;
    }
[ForeignKey("OrganisationChart")]
        public long? OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("Employee")]
        public long EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }
  

        public long? PrimaryOrSecondaryId { get; set; }
        public long? DrivingConstraintId { get; set; }
        public long? LicenseTypeId { get; set; }
        public long? LicenseValidationPeriodId { get; set; }
        [StringLength(256)]
        public string? LicenseSerialNumber { get; set; } = string.Empty;
        [StringLength(256)]
        public string? PreviousDerivingNumber { get; set; } = string.Empty;
        [StringLength(256)]
        public string? Licencedescription { get; set; } = string.Empty;
        [Column(TypeName = "date")]
        public DateTime? LicenseImplDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? LicensePrimaryImplDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? LicenseExpireDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? Previouslicenseimpldate { get; set; }
        [NotMapped]
        private new string title { get; set; } = string.Empty;
    }
}

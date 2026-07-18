using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs
{
    public class DrivingLicenseDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChartTitle { get; set; }
        public long EmployeeId { get; set; }
        public long? LicenseTypeId { get; set; }
        public string? LicenseTypeTitle { get; set; }
        public long? LicenseValidationPeriodId { get; set; }
        public string? LicenseValidationPeriodTitle { get; set; }
        public long? DrivingConstraintId { get; set; }
        public string? DrivingConstraintTitle { get; set; }
        public long? PrimaryOrSecondaryId { get; set; }
        public string? PrimaryOrSecondaryTitle { get; set; }
        public string? LicenseSerialNumber { get; set; }
        public string? PreviousDerivingNumber { get; set; }
        public string? Licencedescription { get; set; }
        public DateTime? LicenseImplDate { get; set; }
        public DateTime? LicensePrimaryImplDate { get; set; }
        public DateTime? LicenseExpireDate { get; set; }
        public DateTime? Previouslicenseimpldate { get; set; }
    }
}

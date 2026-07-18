using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs
{
    public class ContactInfoDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChartTitle { get; set; }
        public long EmployeeId { get; set; }
        public long? AddressTypeId { get; set; }
        public string? AddressTypeTitle { get; set; }
        public string? Address { get; set; }
        public string? Zipcode { get; set; }
        public string? Phone { get; set; }
        public string? EmergencyPhone { get; set; }
        public string? Fax { get; set; }
        public string? Mail { get; set; }
        public long? LocationTypeId { get; set; }
        public string?  LocationTypeTitle { get; set; }
        public string? MobileNo { get; set; }
        public bool IsVerify { get; set; }
        public bool IsLast { get; set; }
    }
}

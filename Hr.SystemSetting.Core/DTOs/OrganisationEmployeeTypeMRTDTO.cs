using HR.SharedKernel.Attribute;
using Hr.SystemSetting.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.DTOs
{
    public class OrganisationEmployeeTypeMRTDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long EmployeeTypeId { get; set; }
        public string? EmployeeType { get; set; }
        public long OrganisationMRTId { get; set; }
        public string? OrganisationMRT { get; set; }
        public bool IsRaw { get; set; }
        public bool IsBatch { get; set; }
        public bool IsManager { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public long? SettingTypeId { get; set; }
        public string? SettingType { get; set; }
    }
}

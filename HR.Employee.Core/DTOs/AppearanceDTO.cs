using HR.SharedKernel.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.SharedKernel.Data;

namespace HR.Employee.Core.DTOs
{
    public class AppearanceDTO : BaseDTO
    {
        public long? OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long EmployeeId { get; set; }
        public string? Employee { get; set; }
        public long? EyeColorId { get; set; }
        public string? EyeColor { get; set; }
        public long? SkinColorId { get; set; }
        public string? SkinColor { get; set; }
        public long? HairColorId { get; set; }
        public string? HairColor { get; set; }
        [StringLength(256)]
        public string? SpecificSymptoms { get; set; }
        public int Weight { get; set; }
        public int Height { get; set; }
        public int FootSize { get; set; }
    }
}

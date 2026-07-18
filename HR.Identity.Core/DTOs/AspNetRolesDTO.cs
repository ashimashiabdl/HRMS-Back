using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.DTOs
{
    public class AspNetRolesDTO : BaseDTO
    {
        [StringLength(256, ErrorMessage = "طول فیلد عنوان می تواند حد اکثر 256 کاراکتر باشد")]
        public string? PersianName { get; set; }
        public string? Name { get; set; }
        //public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public bool Disabled { get; set; }
    }
}

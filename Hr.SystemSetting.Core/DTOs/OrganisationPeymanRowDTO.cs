using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.DTOs
{
    public class OrganisationPeymanRowDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public  string? OrganisationChart { get; set; }
        /// <summary>
        /// کد ردیف پیمان
        /// </summary>
        [StringLength(32)]
        public string? Code { get; set; }
    }
}

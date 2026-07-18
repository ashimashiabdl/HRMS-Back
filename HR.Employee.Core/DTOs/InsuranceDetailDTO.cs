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
    public class InsuranceDetailDTO : BaseDTO
    {
        public long InsuranceId { get; set; }
        public string? Insurance { get; set; }

        public DateTime? Date { get; set; }

        public long? StatusId { get; set; }

        public string? Status { get; set; }
        public long? InsuranceTypeRecordId { get; set; }
        public string? InsuranceTypeRecord { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? AccDay { get; set; }
        public DateTime? InsuranceStartDate { get; set; }
        public DateTime? InsuranceEndDate { get; set; }
        public bool? IsFullInsurnce { get; set; }
        public bool? IsComputable { get; set; }
        public bool? IsOptionalInsurnce { get; set; }
        [StringLength(256)]
        public string? Desc { get; set; }


    }
}

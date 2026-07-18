using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs
{
    public class FilterBatchDTO
    {
        public long? EmployeeTypeId { get; set; }
        public long OrderTypeId { get; set; }
        public long? CostCenterId { get; set; }
        public long? OrganizationUnitId { get; set; }
        public long? WorkPlaceId { get; set; }
        public long? EmployeeStatusId { get; set; }
        public string? NationalNos { get; set; }
        [Required]
        public DateTime ImpleDate { get; set; }

        /// <summary>1-based page number for preview grid.</summary>
        public int PageNo { get; set; } = 1;

        public int PageSize { get; set; } = 25;

        /// <summary>PersonelCode | FirstName | LastName | NationalNo</summary>
        public string SortColumn { get; set; } = "PersonelCode";

        /// <summary>ASC | DESC</summary>
        public string SortOrder { get; set; } = "ASC";

        public string? SearchText { get; set; }

        /// <summary>When true, returns all matching rows (used on submit).</summary>
        public bool ReturnAll { get; set; }
    }
}

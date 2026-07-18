using HR.Payroll.Core.Data;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class InsuranceBranchDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long InsuranceTypeId { get; set; }
        public string? InsuranceType { get; set; }
        [StringLength(512)]
        public string? WorkshopName { get; set; }
        [StringLength(512)]
        public string? WorkshopCode { get; set; }
        [StringLength(512)]
        public string? WorkshopPhone { get; set; }
        [StringLength(512)]
        public string? WorkshopAddress { get; set; }
        public bool? IsActive { get; set; }
        [StringLength(512)]
        public string? EmployerName { get; set; }
    }
}

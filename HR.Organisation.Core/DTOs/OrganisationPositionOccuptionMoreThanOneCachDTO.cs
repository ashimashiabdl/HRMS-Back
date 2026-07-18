using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.DTOs
{
    public class OrganisationPositionOccuptionMoreThanOneCachDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        [MaxLength(255)]
        public string Position { get; set; } = null!;
        [MaxLength(30)]
        public string? PositionCode { get; set; }
        public int? Count { get; set; }
        [MaxLength(255)]
        public string? Job { get; set; }
        [MaxLength(10)]
        public string? JobCode { get; set; }
        [MaxLength(50)]
        public string? PersonelCode { get; set; }
        [MaxLength(100)]
        public string? FirstName { get; set; }
        [MaxLength(100)]
        public string? LastName { get; set; }
        [MaxLength(255)]
        public string? WorkPlace { get; set; }
    }
}

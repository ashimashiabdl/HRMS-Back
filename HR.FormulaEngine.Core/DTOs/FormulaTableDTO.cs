using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.FormulaEngine.Core.DTOs
{
    public class FormulaTableDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long TableTypeId { get; set; }
        public string? TableType { get; set; }
        public int Rank { get; set; }
        [StringLength(256)]
        public string? RelatedContextField { get; set; }
        public bool SetZeroIfNotFound { get; set; }
        [StringLength(512)]
        public string? Description { get; set; }
    }
}

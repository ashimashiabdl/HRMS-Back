using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.FormulaEngine.Core.DTOs
{
    public class FormulaDatabaseFunctionDefinitionDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long FuctionTypeId { get; set; }
        public string? FuctionType { get; set; }
        [StringLength(256)]
        public string? EnglishName { get; set; }
        [StringLength(32)]
        public string? Schema { get; set; }
        [StringLength(255)]
        public string? FunctionName { get; set; }
        [StringLength(1024)]
        public string? Help { get; set; }
        public string? ParamsJson { get; set; }
        public string? Body { get; set; }
        public int NumberOfParameters { get; set; }
        public bool IsPublic { get; set; }
    }


 
}

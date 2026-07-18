using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class ArearsChangedFicheItemsDTO : BaseDTO
    {
        public long ArearFicheId { get; set; }
        public string? ArearFiche { get; set; }
        public long WageItemId { get; set; }
        public string? WageItem { get; set; }
        public long CurrentAmount { get; set; }
        /// <summary>
        /// مبلغ در فیش قبلی
        /// </summary>
        [Comment("مبلغ فیش قبلی")]
        public long LastAmount { get; set; }
    }
}

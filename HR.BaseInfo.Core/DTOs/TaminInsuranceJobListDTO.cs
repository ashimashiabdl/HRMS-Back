using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.DTOs
{
    public class TaminInsuranceJobListDTO : BaseDTO
    {
        [MaxLength(6)]
        public string? Code { get; set; }
    }
}

using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.DTOs
{
    public class BaseTableValueDTO : BaseDTO
    {
        public string? Description { get; set; }
        public long BaseTableId { get; set; }
        public  string? BaseTableTitle { get; set; }
        public int Order { get; set; }
        public string? Value { get; set; }
        public bool Visible { get; set; }
     
    }
}

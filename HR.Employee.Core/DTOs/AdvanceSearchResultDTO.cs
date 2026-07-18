using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs
{
    public class AdvanceSearchResultDTO
    {
        public long Id { get; set; }
    
        public string? FirstName { get; set; }

        public string? LastName { get; set; }
 
        public string? FatherName { get; set; }
     
        public string? PersonelCode { get; set; }
    }
}

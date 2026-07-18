using HR.SharedKernel.Attribute;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Data
{
    public class BaseViewEntity
    {
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long Id { get; set; }
        public string title { get; set; }
   
        public DateTime? CreateDate { get; set; }
    
        public DateTime? LastModifiedDate { get; set; }

  
        public string IPAddress { get; set; }
        public bool IsDeleted { get; set; }
        
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}

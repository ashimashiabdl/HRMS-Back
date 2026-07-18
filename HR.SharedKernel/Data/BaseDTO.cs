using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Data
{
    public class BaseDTO
    {
        public long? Id { get; set; }
        public string? title { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? StartDate { get; set; }

        public Nullable<DateTime> EndDate { get; set; }
    }
}

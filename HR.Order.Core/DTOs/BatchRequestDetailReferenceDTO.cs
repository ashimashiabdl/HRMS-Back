using HR.Order.Core.Data;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs
{
    public class BatchRequestDetailReferenceDTO : BaseDTO
    {
        public long BatchRequestDetailId { get; set; }
        public string? BatchRequestDetail { get; set; }
        [StringLength(128)]
        public string? FinalMessage { get; set; }
        public DateTime DoDatetime { get; set; }
        public DateTime LastTryDateTime { get; set; }
    }
}

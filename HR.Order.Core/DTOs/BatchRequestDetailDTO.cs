using HR.Order.Core.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.SharedKernel.Data;

namespace HR.Order.Core.DTOs
{
    public class BatchRequestDetailDTO : BaseDTO
    {
        public long EmployeeId { get; set; }
        public string? Employee { get; set; }
        public string? NationalNo { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PersonelCode { get; set; }
        public long BatchRequestId { get; set; }
        public string? BatchRequest { get; set; }
        public string? OrderStatus { get; set; }
        public long? InterdictId { get; set; }
        public long? StatusId { get; set; }
        public string? InterdictOrder { get; set; }
        [StringLength(128)]
        public string? FinalMessage { get; set; }
        [StringLength(8096)]
        public string? PdfrawByteArrayFinalMessage { get; set; }
        [StringLength(8096)]
        public string? PdfByteArrayFinalMessage { get; set; }
        [StringLength(128)]
        public string? ArchiveFinalMessage { get; set; }
        public DateTime DoDatetime { get; set; }
        public DateTime LastTryDateTime { get; set; }
    }
}

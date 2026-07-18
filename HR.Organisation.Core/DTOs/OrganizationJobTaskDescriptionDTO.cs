using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.DTOs
{
    public class OrganizationJobTaskDescriptionDTO : BaseDTO
    {
        public long? OrganizationJobId { get; set; }
        public string? OrganizationJob { get; set; }
        public long TaskTypeId { get; set; }
        public string? TaskType { get; set; }
        [MaxLength(8096)]
        public string? TaskDescription { get; set; }
    }
}

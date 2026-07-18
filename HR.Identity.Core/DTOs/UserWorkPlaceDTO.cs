using HR.Identity.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.DTOs
{
    public class UserWorkPlaceDTO : BaseDTO
    {
        public long? UserId { get; set; }
        public string? User { get; set; }
        public long? WorkPlaceId { get; set; }
        public string? WorkPlace { get; set; }
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
    }
}

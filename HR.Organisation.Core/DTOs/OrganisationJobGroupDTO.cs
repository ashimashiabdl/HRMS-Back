using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.SharedKernel.Data;

namespace HR.Organisation.Core.DTOs
{
    public class OrganisationJobGroupDTO : BaseDTO
    {
        public long? OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long? StateId { get; set; }
        public string? State { get; set; }
        public long? JobGroupId { get; set; }
        public string? JobGroup { get; set; }
        public long OrganisationJobCategoryId { get; set; }
        public string? OrganisationJobCategory { get; set; }
        [StringLength(50)]
        public string? Code { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public int Order { get; set; }
    }
}

using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.DTOs
{
    public class OrganisationJobSeriesDTO : BaseDTO
    {
        public long? OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long? JobSeriesId { get; set; }
        public string? JobSeries { get; set; }
        public long OrganisationJobCategoryId { get; set; }
        public string? OrganisationJobCategory { get; set; }
        public long OrganisationJobGroupId { get; set; }
        public string? OrganisationJobGroup { get; set; }
        [StringLength(50)]
        public string? Code { get; set; }
    }
}

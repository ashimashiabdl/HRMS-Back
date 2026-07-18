using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Data
{
    /// <summary>
    /// هر کلاس که از این واسط ارث بری کند ستون مورد نظر هیچ گاه بروز نمی شود
    /// </summary>
    public interface IOrganisationChartId
    {
        public long OrganisationChartId { get; set; }
    }
}

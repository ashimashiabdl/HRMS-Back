using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.DTOs
{
    public class AgentOfPunishmentEncourageDTO : BaseDTO
    {
        /// <summary>
        /// آیا این عامل تنبیهی است ؟
        /// </summary>
        public bool IsPunishment { get; set; }
        /// <summary>
        /// امتیاز به ازای هر واحد
        /// </summary>
        //public int PointsPerUnit { get; set; }
    }
}
